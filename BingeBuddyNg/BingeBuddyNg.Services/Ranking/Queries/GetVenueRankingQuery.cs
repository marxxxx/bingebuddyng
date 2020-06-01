using BingeBuddyNg.Services.Activity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Ranking.Querys
{
    public class GetVenueRankingQuery : IRequest<IEnumerable<VenueRankingDTO>>
    {
    }

    public class GetVenueRankingQueryHandler : IRequestHandler<GetVenueRankingQuery, IEnumerable<VenueRankingDTO>>
    {
        private readonly IActivityRepository activityRepository;

        public GetVenueRankingQueryHandler(IActivityRepository activityRepository)
        {
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        public async Task<IEnumerable<VenueRankingDTO>> Handle(GetVenueRankingQuery request, CancellationToken cancellationToken)
        {
            var args = new GetActivityFilterArgs() { FilterOptions = ActivityFilterOptions.WithVenue, PageSize = 100, ActivityType = ActivityType.Drink };
            var activitys = await activityRepository.GetMasterActivitiesAsync(args);
            var result = activitys.GroupBy(r => new { r.Venue.Id, r.Venue.Name })
                .Select(r => new VenueRankingDTO(r.Key.Id, r.Key.Name, r.Count()))
                .OrderByDescending(r => r.Count);
            return result;
        }
    }
}
