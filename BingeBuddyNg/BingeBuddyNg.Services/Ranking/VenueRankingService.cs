using BingeBuddyNg.Services.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Ranking
{
    public class VenueRankingService : IVenueRankingService
    {
        public IActivityRepository ActivityRepository { get; }

        public VenueRankingService(IActivityRepository activityRepository)
        {
            ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }
        
        public async Task<IEnumerable<VenueRanking>> GetVenueRankingAsync()
        {
            var activitys = await ActivityRepository.GetActivityFeedAsync(new GetActivityFilterArgs(ActivityFilterOptions.WithVenue, pageSize: 100, activityType: ActivityType.Drink));
            var result = activitys.ResultPage.GroupBy(r => new { r.Venue.Id, r.Venue.Name })
                .Select(r => new VenueRanking(r.Key.Id, r.Key.Name, r.Count()))
                .OrderByDescending(r=>r.Count);
            return result;

        }

        public Task<IEnumerable<VenueRanking>> GetVenueRankingForUserAsync()
        {
            throw new NotImplementedException();
        }
    }
}
