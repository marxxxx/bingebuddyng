using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Ranking
{
    public interface IVenueRankingService
    {
        Task<IEnumerable<VenueRanking>> GetVenueRankingAsync();
        Task<IEnumerable<VenueRanking>> GetVenueRankingForUserAsync();        
    }
}
