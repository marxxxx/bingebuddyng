using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.Venue.DTO;
using MediatR;

namespace BingeBuddyNg.Core.Venue.Queries
{
    public class SearchVenuesQuery : IRequest<List<VenueDTO>>
    {
        public SearchVenuesQuery(float latitude, float longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public float Latitude { get; }
        public float Longitude { get; }
    }

    public class SearchVenuesQueryHandler : IRequestHandler<SearchVenuesQuery, List<VenueDTO>>
    {
        private readonly IFourSquareService fourSquareService;

        public SearchVenuesQueryHandler(IFourSquareService fourSquareService)
        {
            this.fourSquareService = fourSquareService;
        }

        public async Task<List<VenueDTO>> Handle(SearchVenuesQuery request, CancellationToken cancellationToken)
        {
            var venueResult = await this.fourSquareService.SearchVenuesAsync(request.Latitude, request.Longitude);

            var venues = venueResult
                .Select(v => new Venue(v.Id, new Activity.Domain.Location(v.Location.Lat, v.Location.Lng), v.Name, v.Location.Distance).ToDto())
                .OrderBy(v => v.Distance)
                .ToList();
            return venues;
        }
    }
}
