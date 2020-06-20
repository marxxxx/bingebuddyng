using System.Threading.Tasks;
using BingeBuddyNg.Core.User.Commands;
using BingeBuddyNg.Services.User.Persistence;

namespace BingeBuddyNg.Core.User
{
    public interface IUserRepository
    {
        Task<User> GetUserAsync(string id);

        Task UpdateUserAsync(UserEntity user);

        Task<CreateOrUpdateUserResult> CreateOrUpdateUserAsync(CreateOrUpdateUserCommand user);
    }
}
