using System;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Activity.Persistence;
using BingeBuddyNg.Services.Activity.Persistence;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Infrastructure.EventGrid;
using BingeBuddyNg.Shared;
using Microsoft.WindowsAzure.Storage.Table;

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

            return activity;
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

        public async Task<Core.Activity.Domain.Activity> GetActivityAsync(string id)
        {
            var tableEntity = await GetActivityEntityAsync(id);
            var entity = tableEntity.Entity;
            return entity.ToDomain();
        }

        public async Task<ActivityTableEntity> GetActivityEntityAsync(string id)
        {
            string partitionKey = ActivityKeyFactory.GetPartitionKeyFromRowKey(id);
            var table = this.storageAccessService.GetTableReference(Constants.TableNames.Activity);

            TableOperation retrieveOperation = TableOperation.Retrieve<ActivityTableEntity>(partitionKey, id);

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
        
        public async Task AddToActivityAddedTopicAsync(string activityId)
        {
            await this.eventGridService.PublishAsync("ActivityAdded", new ActivityAddedMessage(activityId));
        }
    }
}
