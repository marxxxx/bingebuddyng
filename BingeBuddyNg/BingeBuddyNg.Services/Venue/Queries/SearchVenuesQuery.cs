using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Venue.DTO;
using BingeBuddyNg.Services.Venue.Generated;
using MediatR;
using Newtonsoft.Json;

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
        private const string BaseUrl = "https://api.foursquare.com/v2";

        private readonly IHttpClientFactory httpClientFactory;
        private readonly FourSquareConfiguration configuration;

        public SearchVenuesQueryHandler(IHttpClientFactory httpClientFactory, FourSquareConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<List<VenueDTO>> Handle(SearchVenuesQuery request, CancellationToken cancellationToken)
        {
            var client = this.httpClientFactory.CreateClient();

            // force english format
            var englishCulture = CultureInfo.GetCultureInfo("en");
            string lalong = $"{request.Latitude.ToString(englishCulture)},{request.Longitude.ToString(englishCulture)}";
            string url = $"{BaseUrl}/venues/search?v=20190112&client_id={this.configuration.FourSquareApiClientKey}&client_secret={this.configuration.FourSquareApiClientSecret}&ll={lalong}";

            var response = await client.GetStringAsync(url);

            // convert
            var venueResult = JsonConvert.DeserializeObject<VenueRootObject>(response);

            var venues = venueResult.Response.Venues
                .Select(v => new Venue(v.Id, new Activity.Domain.Location(v.Location.Lat, v.Location.Lng), v.Name, v.Location.Distance).ToDto())
                .OrderBy(v => v.Distance)
                .ToList();
            return venues;
        }
    }
}
