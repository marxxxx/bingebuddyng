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
        Task<List<Activity>> GetActivitysForUser(string userId);
        Task AddActivityAsync(Activity activity);
    }
}
