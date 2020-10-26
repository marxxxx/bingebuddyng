using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Core.User.Commands;
using BingeBuddyNg.Core.User.Persistence;

namespace BingeBuddyNg.Core.User
{
    public interface IUserRepository
    {
        Task<Domain.User> GetUserAsync(string id);

        Task UpdateUserAsync(UserEntity user);

        Task<CreateOrUpdateUserResult> CreateOrUpdateUserAsync(CreateOrUpdateUserCommand user);

        Task<IEnumerable<string>> GetAllUserIdsAsync();

        Task<List<UserEntity>> SearchUsersAsync(IEnumerable<string> userIds = null, string filterText = null);
    }
}
