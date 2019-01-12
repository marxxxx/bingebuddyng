using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Interfaces
{
    public interface IVenueUserRepository
    {
        Task AddUserToVenueAsync(string venueId, string venueName, string userId, string userName);
        Task RemoveUserFromVenueAsync(string venueId, string userId);
    }
}
