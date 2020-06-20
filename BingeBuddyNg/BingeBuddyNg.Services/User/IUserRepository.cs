using BingeBuddyNg.Services.User.Commands;
using BingeBuddyNg.Services.User.Persistence;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.User
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserEntity>> GetUsersAsync(IEnumerable<string> userIds = null);

        Task<IEnumerable<string>> GetAllUserIdsAsync();

        Task<User> GetUserAsync(string id);

        Task UpdateUserAsync(UserEntity user);

        Task<CreateOrUpdateUserResult> CreateOrUpdateUserAsync(CreateOrUpdateUserCommand user);

        Task UpdateMonitoringInstanceAsync(string userId, string monitoringInstanceId);
    }
}
