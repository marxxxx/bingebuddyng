using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity;

namespace BingeBuddyNg.Services.Infrastructure
{
    public interface IUtilityService
    {
        Task<AddressInfo> GetAddressFromLongLatAsync(Location location);
    }
}
