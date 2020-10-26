using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.Activity.Persistence;

namespace BingeBuddyNg.Core.Activity
{
    public interface IActivityRepository
    {
        Task<IEnumerable<ActivityEntity>> GetMasterActivitiesAsync(ActivityFilterArgs args);

        Task<IEnumerable<ActivityEntity>> GetUserActivitiesAsync(string userId, DateTime startTimeUtc, ActivityType activityType = ActivityType.None);

        Task<Domain.Activity> GetActivityAsync(string id);

        Task<ActivityEntity> AddActivityAsync(ActivityEntity activity);

        Task AddToPersonalizedFeedAsync(IEnumerable<string> userIds, ActivityEntity activity);

        Task AddToPersonalizedFeedAsync(string userId, ActivityEntity activity);

        Task DeleteActivityAsync(string userId, string id);

        Task DeleteFromPersonalizedFeedAsync(string userId, string id);

        Task UpdateActivityAsync(ActivityEntity activity);

        Task UpdateActivityAsync(string userId, ActivityEntity activity);

        Task AddToActivityAddedTopicAsync(string activityId);
    }
}
