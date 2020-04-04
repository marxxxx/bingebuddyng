using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Venue
{
    public interface IVenueUserRepository
    {
        Task AddUserToVenueAsync(string venueId, string venueName, string userId, string userName);

        Task RemoveUserFromVenueAsync(string venueId, string userId);
    }
}
