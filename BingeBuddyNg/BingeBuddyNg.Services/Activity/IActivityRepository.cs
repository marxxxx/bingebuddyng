using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Activity
{
    public interface IActivityRepository
    {
        Task<IEnumerable<ActivityDTO>> GetMasterActivitiesAsync(GetActivityFilterArgs args);

        Task<IEnumerable<ActivityDTO>> GetUserActivitiesAsync(string userId, DateTime startTimeUtc, ActivityType activityType = ActivityType.None);

        Task<Activity> GetActivityAsync(string id);

        Task<Activity> AddActivityAsync(Activity activity);

        Task DistributeActivityAsync(IEnumerable<string> distributionUserIds, Activity activity);

        Task DeleteActivityAsync(string userId, string id);

        Task DeleteActivityFromPersonalizedFeedAsync(string userId, string id);

        Task UpdateActivityAsync(Activity activity);

        Task AddToActivityAddedTopicAsync(string activityId);
    }
}
