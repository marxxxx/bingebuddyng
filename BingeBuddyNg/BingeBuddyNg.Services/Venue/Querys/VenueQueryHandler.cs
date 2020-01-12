using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Venue.Generated;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Venue.Querys
{
    public class VenueQueryHandler : IRequestHandler<SearchVenuesQuery, List<VenueModel>>
    {
        private const string BaseUrl = "https://api.foursquare.com/v2";

        public IHttpClientFactory HttpClientFactory { get; }
        public FourSquareConfiguration Configuration { get; }

        public VenueQueryHandler(IHttpClientFactory httpClientFactory, FourSquareConfiguration configuration)
        {
            HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }        

        public async Task<List<VenueModel>> Handle(SearchVenuesQuery request, CancellationToken cancellationToken)
        {
            var client = this.HttpClientFactory.CreateClient();

            // force english format
            var englishCulture = CultureInfo.GetCultureInfo("en");
            string lalong = $"{request.Latitude.ToString(englishCulture)},{request.Longitude.ToString(englishCulture)}";
            string url = $"{BaseUrl}/venues/search?v=20190112&client_id={this.Configuration.FourSquareApiClientKey}&client_secret={this.Configuration.FourSquareApiClientSecret}&ll={lalong}";

            var response = await client.GetStringAsync(url);

            // convert
            var venueResult = JsonConvert.DeserializeObject<VenueRootObject>(response);

            var venues = venueResult.Response.Venues
                .Select(v => new VenueModel(v.Id, new Activity.Location(v.Location.Lat, v.Location.Lng), v.Name, v.Location.Distance))
                .OrderBy(v => v.Distance)
                .ToList();
            return venues;
        }
    }
}
