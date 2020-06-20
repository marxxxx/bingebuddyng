using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Services.User.Persistence;

namespace BingeBuddyNg.Services.User.Queries
{
    public interface ISearchUsersQuery
    {
        Task<List<UserEntity>> ExecuteAsync(IEnumerable<string> userIds = null, string filterText = null);
    }
}