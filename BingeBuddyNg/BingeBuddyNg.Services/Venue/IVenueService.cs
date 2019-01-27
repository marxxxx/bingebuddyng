using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Venue
{
    public interface IVenueService
    {
        Task<List<VenueModel>> SearchVenuesAsync(float latitude, float longitude);
        Task UpdateVenueForUserAsync(string userId, VenueModel venue);
        Task LeaveVenueForUserAsync(string userId);
    }
}
