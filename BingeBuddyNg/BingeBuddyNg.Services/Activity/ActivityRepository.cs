﻿using System;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Activity.Persistence;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Shared;

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
            string activityFeedRowKey = ActivityKeyFactory.CreateRowKey(activity.Timestamp, activity.UserId);
            var entity = new ActivityTableEntity(ActivityKeyFactory.CreatePartitionKey(activity.Timestamp), activityFeedRowKey, activity);

            await this.storageAccessService.InsertAsync(Constants.TableNames.Activity, entity);

            string activityPerUserRowKey = ActivityKeyFactory.CreatePerUserRowKey(activity.Timestamp);
            var perUserEntity = new ActivityTableEntity(activity.UserId, activityPerUserRowKey, activity);

            await this.storageAccessService.InsertAsync(Constants.TableNames.ActivityPerUser, perUserEntity);

            return activity;
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
            return await this.storageAccessService.GetTableEntityAsync<ActivityTableEntity>(Constants.TableNames.Activity, partitionKey, id);
        }

        public async Task UpdateActivityAsync(ActivityEntity activity)
        {
            ActivityTableEntity entity = await GetActivityEntityAsync(activity.Id);
            entity.Entity = activity;

            await this.storageAccessService.ReplaceAsync(Constants.TableNames.Activity, entity);
        }
        
        public async Task AddToActivityAddedTopicAsync(string activityId)
        {
            await this.eventGridService.PublishAsync("ActivityAdded", new ActivityAddedMessage(activityId));
        }
    }
}
