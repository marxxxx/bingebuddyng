using BingeBuddyNg.Services.Models.Venue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Interfaces
{
    public interface IFourSquareService
    {
        Task<List<VenueModel>> SearchVenuesAsync(float latitude, float longitude);
    }
}
