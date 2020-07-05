using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity.Domain;

namespace BingeBuddyNg.Core.Infrastructure
{
    public interface IAddressDecodingService
    {
        Task<AddressInfo> GetAddressFromLongLatAsync(Location location);
    }
}
