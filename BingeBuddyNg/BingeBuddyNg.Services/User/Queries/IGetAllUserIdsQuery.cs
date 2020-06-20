using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingeBuddyNg.Core.User.Queries
{
    public interface IGetAllUserIdsQuery
    {
        Task<IEnumerable<string>> ExecuteAsync();
    }
}