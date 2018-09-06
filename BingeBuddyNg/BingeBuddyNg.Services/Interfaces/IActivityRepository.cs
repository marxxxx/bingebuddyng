using BingeBuddyNg.Services.DTO;
using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static BingeBuddyNg.Services.StorageAccessService;

namespace BingeBuddyNg.Services.Interfaces
{
    public interface IActivityRepository
    {
        Task<PagedQueryResult<Activity>> GetActivityFeedAsync(GetActivityFilterArgs args);
        Task<List<Activity>> GetActivitysForUser(string userId, DateTime startTimeUtc, ActivityType activityType);
        Task<Activity> GetActivityAsync(string id);
        Task<Activity> AddActivityAsync(Activity activity);

        Task UpdateActivityAsync(Activity activity);
    }
}
