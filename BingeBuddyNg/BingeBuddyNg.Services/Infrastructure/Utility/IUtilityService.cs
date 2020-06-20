using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity.Domain;

namespace BingeBuddyNg.Services.Infrastructure
{
    public interface IUtilityService
    {
        Task<AddressInfo> GetAddressFromLongLatAsync(Location location);
    }
}
