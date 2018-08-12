using BingeBuddyNg.Services.DTO;
using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Interfaces
{
    public interface IActivityRepository
    {
        Task<List<Activity>> GetActivitysAsync();
        Task<List<Activity>> GetActivitysForUser(string userId, DateTime startTimeUtc, ActivityType activityType);
        Task<Activity> GetActivityAsync(string userId, string id);
        Task<Activity> AddActivityAsync(Activity activity);

        Task UpdateActivityAsync(Activity activity);
    }
}
