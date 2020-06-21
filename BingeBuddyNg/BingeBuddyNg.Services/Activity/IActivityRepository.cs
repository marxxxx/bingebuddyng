using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity.Persistence;

namespace BingeBuddyNg.Core.Activity
{
    public interface IActivityRepository
    {
        Task<Domain.Activity> GetActivityAsync(string id);

        Task<ActivityEntity> AddActivityAsync(ActivityEntity activity);

        Task DeleteActivityAsync(string userId, string id);

        Task UpdateActivityAsync(ActivityEntity activity);

        Task AddToActivityAddedTopicAsync(string activityId);
    }
}
