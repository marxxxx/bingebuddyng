using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Activity.Persistence;

namespace BingeBuddyNg.Core.Activity.Queries
{
    public interface IGetMasterActivitiesQuery
    {
        Task<IEnumerable<ActivityEntity>> ExecuteAsync(ActivityFilterArgs args);
    }
}