using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity.Persistence;

namespace BingeBuddyNg.Services.Activity
{
    public interface IActivityRepository
    {
        Task<IEnumerable<ActivityEntity>> GetMasterActivitiesAsync(GetActivityFilterArgs args);

        Task<IEnumerable<ActivityEntity>> GetUserActivitiesAsync(string userId, DateTime startTimeUtc, ActivityType activityType = ActivityType.None);

        Task<Activity> GetActivityAsync(string id);

        Task<ActivityEntity> AddActivityAsync(ActivityEntity activity);

        Task DistributeActivityAsync(IEnumerable<string> distributionUserIds, ActivityEntity activity);

        Task DeleteActivityAsync(string userId, string id);

        Task DeleteActivityFromPersonalizedFeedAsync(string userId, string id);

        Task UpdateActivityAsync(ActivityEntity activity);

        Task AddToActivityAddedTopicAsync(string activityId);
    }
}
