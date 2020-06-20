using System.Threading.Tasks;
using BingeBuddyNg.Services.User.Commands;
using BingeBuddyNg.Services.User.Persistence;

namespace BingeBuddyNg.Services.User
{
    public interface IUserRepository
    {
        Task<User> GetUserAsync(string id);

        Task UpdateUserAsync(UserEntity user);

        Task<CreateOrUpdateUserResult> CreateOrUpdateUserAsync(CreateOrUpdateUserCommand user);

        Task UpdateMonitoringInstanceAsync(string userId, string monitoringInstanceId);
    }
}
