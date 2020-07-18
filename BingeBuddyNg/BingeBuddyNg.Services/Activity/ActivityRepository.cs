using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity.Messages;
using BingeBuddyNg.Core.Activity.Persistence;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Shared;
using Microsoft.Extensions.Logging;

namespace BingeBuddyNg.Core.Activity
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly IStorageAccessService storageAccessService;
        private readonly IEventGridService eventGridService;
        private readonly ILogger<ActivityRepository> logger;

        public ActivityRepository(
            IStorageAccessService storageAccessService,
            IEventGridService eventGridService,
            ILogger<ActivityRepository> logger)
        {
            this.storageAccessService = storageAccessService;
            this.eventGridService = eventGridService;
            this.logger = logger;
        }

        public async Task<ActivityEntity> AddActivityAsync(ActivityEntity activity)
        {
            string activityFeedRowKey = ActivityKeyFactory.CreateRowKey(activity.Timestamp, activity.UserId);
            var entity = new ActivityTableEntity(ActivityKeyFactory.CreatePartitionKey(activity.Timestamp), activityFeedRowKey, activity);
            await this.storageAccessService.InsertAsync(Constants.TableNames.Activity, entity);

            string activityPerUserRowKey = ActivityKeyFactory.CreatePerUserRowKey(activity.Timestamp);
            var perUserEntity = new ActivityTableEntity(activity.UserId, activityPerUserRowKey, activity);
            await this.storageAccessService.InsertAsync(Constants.TableNames.ActivityPerUser, perUserEntity);

            await AddToPersonalizedFeedAsync(activity.UserId, activity);

            return activity;
        }

        public async Task AddToPersonalizedFeedAsync(IEnumerable<string> userIds, ActivityEntity activity)
        {
            foreach (var userId in userIds)
            {
                try
                {
                    await AddToPersonalizedFeedAsync(userId, activity);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Failed to distribute activity to [{userId}].");
                }
            }
        }

        public async Task AddToPersonalizedFeedAsync(string userId, ActivityEntity activity)
        {
            var personalEntity = new ActivityTableEntity(userId, activity.Id, activity);
            await this.storageAccessService.InsertOrMergeAsync(Constants.TableNames.ActivityUserFeed, personalEntity);
        }

        public async Task DeleteActivityAsync(string userId, string id)
        {
            var activity = await this.GetActivityEntityAsync(id);
            if (activity != null)
            {
                if (string.Compare(activity.UserId, userId, true) != 0)
                {
                    throw new UnauthorizedAccessException($"User {userId} is not permitted to delete an activity of user {activity.UserId}");
                }

                await storageAccessService.DeleteAsync(Constants.TableNames.Activity, activity);

                // Delete activity in per-user table as well
                var perUserActivity = await this.GetActivityPerUserEntityAsync(userId, activity.Entity.Timestamp);
                if (perUserActivity != null)
                {
                    await storageAccessService.DeleteAsync(Constants.TableNames.ActivityPerUser, perUserActivity);
                }
            }

            await DeleteFromPersonalizedFeedAsync(userId, id);
        }

        public async Task DeleteFromPersonalizedFeedAsync(string userId, string id)
        {
            var userFeedResult = await this.storageAccessService.GetTableEntityAsync<ActivityTableEntity>(Constants.TableNames.ActivityUserFeed, userId, id);
            if (userFeedResult != null)
            {
                await storageAccessService.DeleteAsync(Constants.TableNames.ActivityUserFeed, userFeedResult);
            }
        }

        private async Task<ActivityTableEntity> GetActivityPerUserEntityAsync(string userId, DateTime timestamp)
        {
            var rowKey = ActivityKeyFactory.CreatePerUserRowKey(timestamp);
            var result = await this.storageAccessService.GetTableEntityAsync<ActivityTableEntity>(Constants.TableNames.ActivityPerUser, userId, rowKey);
            return result;
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
            var entity = await this.storageAccessService.GetTableEntityAsync<ActivityTableEntity>(Constants.TableNames.Activity, partitionKey, id);
            if (entity == null)
            {
                throw new NotFoundException($"Activity [{id}] not found!");
            }

            return entity;
        }

        public async Task UpdateActivityAsync(ActivityEntity activity)
        {
            ActivityTableEntity entity = await GetActivityEntityAsync(activity.Id);
            entity.Entity = activity;

            await this.storageAccessService.ReplaceAsync(Constants.TableNames.Activity, entity);
        }

        public async Task UpdateActivityAsync(string userId, ActivityEntity activity)
        {
            await UpdateActivityAsync(activity);

            var userFeedResult = await this.storageAccessService.GetTableEntityAsync<ActivityTableEntity>(Constants.TableNames.ActivityUserFeed, userId, activity.Id);
            if (userFeedResult != null)
            {
                userFeedResult.Entity = activity;
                await this.storageAccessService.ReplaceAsync(Constants.TableNames.ActivityUserFeed, userFeedResult);
            }
        }

        public async Task AddToActivityAddedTopicAsync(string activityId)
        {
            await this.eventGridService.PublishAsync("ActivityAdded", new ActivityAddedMessage(activityId));
        }

    }
}
