using BingeBuddyNg.Services.User.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.User
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsersAsync(IEnumerable<string> userIds = null);

        Task<IEnumerable<string>> GetAllUserIdsAsync();

        Task<User> FindUserAsync(string id);

        Task UpdateUserAsync(User user);

        Task<CreateOrUpdateUserResult> CreateOrUpdateUserAsync(CreateOrUpdateUserCommand user);

        Task UpdateMonitoringInstanceAsync(string userId, string monitoringInstanceId);

        Task AddFriendAsync(string userId, string friendUserId);

        Task RemoveFriendAsync(string userId, string friendUserId);
    }
}
