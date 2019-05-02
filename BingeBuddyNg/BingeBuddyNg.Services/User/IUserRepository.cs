using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.User
{
    public interface IUserRepository
    {
        Task<List<User>> GetUsersAsync(IEnumerable<string> userIds = null);
        Task<User> FindUserAsync(string id);
        Task UpdateUserAsync(User user);
        Task<CreateOrUpdateUserResult> CreateOrUpdateUserAsync(User user);

        Task UpdateMonitoringInstanceAsync(string userId, string monitoringInstanceId);

        Task AddFriendAsync(string userId, string friendUserId);
        Task RemoveFriendAsync(string userId, string friendUserId);
    }
}
