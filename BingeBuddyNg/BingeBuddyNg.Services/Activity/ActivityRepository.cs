using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Activity.Persistence;
using BingeBuddyNg.Services.Activity.Messages;
using BingeBuddyNg.Services.Activity.Persistence;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Infrastructure.EventGrid;
using BingeBuddyNg.Services.Venue;
using BingeBuddyNg.Shared;
using Microsoft.WindowsAzure.Storage.Table;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Services.Activity
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly IStorageAccessService storageAccessService;
        private readonly IEventGridService eventGridService;

        public ActivityRepository(
            IStorageAccessService storageAccessService,
            IEventGridService eventGridService)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
            this.eventGridService = eventGridService ?? throw new ArgumentNullException(nameof(eventGridService));
        }       

        public async Task<IEnumerable<ActivityEntity>> GetUserActivitiesAsync(string userId, DateTime startTimeUtc, Core.Activity.Domain.ActivityType activityType = Core.Activity.Domain.ActivityType.None)
        {
            string startRowKey = ActivityKeyFactory.CreatePerUserRowKey(startTimeUtc);
            var whereClause =
                TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, startRowKey));

            if (activityType != Core.Activity.Domain.ActivityType.None)
            {
                whereClause = TableQuery.CombineFilters(whereClause, TableOperators.And,
                    TableQuery.GenerateFilterCondition(nameof(ActivityTableEntity.ActivityType), QueryComparisons.Equal, activityType.ToString()));
            }

            var result = await storageAccessService.QueryTableAsync<JsonTableEntity<ActivityEntity>>(Constants.TableNames.ActivityPerUser, whereClause);

            var activitys = result.Select(a => a.Entity).ToList();
            return activitys;
        }

        public async Task<ActivityEntity> AddActivityAsync(ActivityEntity activity)
        {
            var activityTable = this.storageAccessService.GetTableReference(Constants.TableNames.Activity);

            string activityFeedRowKey = ActivityKeyFactory.CreateRowKey(activity.Timestamp, activity.UserId);
            var entity = new ActivityTableEntity(ActivityKeyFactory.CreatePartitionKey(activity.Timestamp), activityFeedRowKey, activity);

            TableOperation operation = TableOperation.Insert(entity);
            await activityTable.ExecuteAsync(operation);

            var perUserActivityTable = this.storageAccessService.GetTableReference(Constants.TableNames.ActivityPerUser);

            string activityPerUserRowKey = ActivityKeyFactory.CreatePerUserRowKey(activity.Timestamp);
            var perUserEntity = new ActivityTableEntity(activity.UserId, activityPerUserRowKey, activity);

            TableOperation perUserOperation = TableOperation.Insert(perUserEntity);
            await perUserActivityTable.ExecuteAsync(perUserOperation);

            // store in own personalize feed first
            await DistributeActivityAsync(new[] { activity.UserId }, activity);

            return activity;
        }

        public async Task DistributeActivityAsync(IEnumerable<string> distributionUserIds, ActivityEntity activity)
        {
            var userFeedTable = this.storageAccessService.GetTableReference(Constants.TableNames.ActivityUserFeed);

            foreach (var userId in distributionUserIds)
            {
                var entity = new ActivityTableEntity(userId, activity.Id, activity);

                TableOperation operation = TableOperation.InsertOrReplace(entity);
                await userFeedTable.ExecuteAsync(operation);
            }
        }

        public async Task<Core.Activity.Domain.Activity> GetActivityAsync(string id)
        {
            var tableEntity = await GetActivityEntityAsync(id);
            var entity = tableEntity.Entity;
            return entity.ToDomain();
        }

        private async Task<ActivityTableEntity> GetActivityEntityAsync(string id)
        {
            string partitionKey = ActivityKeyFactory.GetPartitionKeyFromRowKey(id);
            var table = this.storageAccessService.GetTableReference(Constants.TableNames.Activity);

            TableOperation retrieveOperation = TableOperation.Retrieve<ActivityTableEntity>(partitionKey, id);

            var result = await table.ExecuteAsync(retrieveOperation);

            var entity = (ActivityTableEntity)result.Result;
            return entity;
        }

        private async Task<ActivityTableEntity> GetActivityPerUserEntityAsync(string userId, DateTime timestamp)
        {
            var table = this.storageAccessService.GetTableReference(Constants.TableNames.ActivityPerUser);
            var rowKey = ActivityKeyFactory.CreatePerUserRowKey(timestamp);

            TableOperation retrieveOperation = TableOperation.Retrieve<ActivityTableEntity>(userId, rowKey);

            var result = await table.ExecuteAsync(retrieveOperation);

            var entity = (ActivityTableEntity)result.Result;
            return entity;
        }

        public async Task UpdateActivityAsync(ActivityEntity activity)
        {
            var table = this.storageAccessService.GetTableReference(Constants.TableNames.Activity);

            ActivityTableEntity entity = await GetActivityEntityAsync(activity.Id);
            entity.Entity = activity;

            TableOperation updateOperation = TableOperation.Replace(entity);
            await table.ExecuteAsync(updateOperation);
        }

        public async Task DeleteActivityAsync(string userId, string id)
        {
            var activityTable = this.storageAccessService.GetTableReference(Constants.TableNames.Activity);
            var activity = await this.GetActivityEntityAsync(id);
            if (activity != null)
            {
                if (string.Compare(activity.UserId, userId, true) != 0)
                {
                    throw new UnauthorizedAccessException($"User {userId} is not permitted to delete an activity of user {activity.UserId}");
                }

                await activityTable.ExecuteAsync(TableOperation.Delete(activity));

                // Delete activity in per-user table as well
                var perUserTable = this.storageAccessService.GetTableReference(Constants.TableNames.ActivityPerUser);
                var perUserActivity = await this.GetActivityPerUserEntityAsync(userId, activity.Entity.Timestamp);
                if (perUserActivity != null)
                {
                    await perUserTable.ExecuteAsync(TableOperation.Delete(perUserActivity));
                }
            }

            // delete from own feed immediately
            await DeleteActivityFromPersonalizedFeedAsync(userId, id);

            // Delete activity in personalized feeds
            await storageAccessService.AddQueueMessage(QueueNames.DeleteActivity, new DeleteActivityMessage(id));
        }

        public async Task DeleteActivityFromPersonalizedFeedAsync(string userId, string id)
        {
            var userFeedTable = this.storageAccessService.GetTableReference(Constants.TableNames.ActivityUserFeed);

            TableOperation retrieveOperation = TableOperation.Retrieve<ActivityTableEntity>(userId, id);
            var result = await userFeedTable.ExecuteAsync(retrieveOperation);

            var entity = (ActivityTableEntity)result.Result;

            if (entity != null)
            {
                await userFeedTable.ExecuteAsync(TableOperation.Delete(entity));
            }
        }

        public async Task AddToActivityAddedTopicAsync(string activityId)
        {
            await this.eventGridService.PublishAsync("ActivityAdded", new ActivityAddedMessage(activityId));
        }
    }
}
