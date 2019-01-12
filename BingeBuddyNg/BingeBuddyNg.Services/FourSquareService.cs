using BingeBuddyNg.Services.Configuration;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models.Generated;
using BingeBuddyNg.Services.Models.Venue;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services
{
    public class FourSquareService : IFourSquareService
    {
        private AppConfiguration configuration;
        private IHttpClientFactory httpClientFactory;
        private ILogger<FourSquareService> logger;

        private const string BaseUrl = "https://api.foursquare.com/v2";

        public FourSquareService(AppConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<FourSquareService> logger)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<VenueModel>> SearchVenuesAsync(float latitude, float longitude)
        {
            var client = this.httpClientFactory.CreateClient();
            
            // force english format
            var englishCulture = CultureInfo.GetCultureInfo("en");
            string lalong = $"{latitude.ToString(englishCulture)},{longitude.ToString(englishCulture)}";
            string url = $"{BaseUrl}/venues/search?v=20190112&client_id={this.configuration.FourSquareApiClientKey}&client_secret={this.configuration.FourSquareApiClientSecret}&ll={lalong}";

            var response = await client.GetStringAsync(url);

            // convert
            var venueResult = JsonConvert.DeserializeObject<VenueRootObject>(response);

            var venues = venueResult.Response.Venues
                .Select(v => new VenueModel(v.Id, new Models.Location(v.Location.Lat, v.Location.Lng), v.Name, v.Location.Distance))
                .OrderBy(v=>v.Distance)
                .ToList();
            return venues;
        }
    }
}
