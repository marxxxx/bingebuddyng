using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity.Persistence;

namespace BingeBuddyNg.Core.Activity
{
    public interface IActivityRepository
    {
        Task<IEnumerable<ActivityEntity>> GetUserActivitiesAsync(string userId, DateTime startTimeUtc, Domain.ActivityType activityType = Domain.ActivityType.None);

        Task<Domain.Activity> GetActivityAsync(string id);

        Task<ActivityEntity> AddActivityAsync(ActivityEntity activity);

        Task DistributeActivityAsync(IEnumerable<string> distributionUserIds, ActivityEntity activity);

        Task DeleteActivityAsync(string userId, string id);

        Task DeleteActivityFromPersonalizedFeedAsync(string userId, string id);

        Task UpdateActivityAsync(ActivityEntity activity);

        Task AddToActivityAddedTopicAsync(string activityId);
    }
}
