using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;

namespace BingeBuddyNg.Services.Activity
{
    public interface IActivityRepository
    {
        Task<PagedQueryResult<Activity>> GetActivityFeedAsync(GetActivityFilterArgs args);
        Task<List<Activity>> GetActivitysForUserAsync(string userId, DateTime startTimeUtc, ActivityType activityType);
        Task<Activity> GetActivityAsync(string id);
        Task<Activity> AddActivityAsync(Activity activity);

        Task UpdateActivityAsync(Activity activity);
    }
}
