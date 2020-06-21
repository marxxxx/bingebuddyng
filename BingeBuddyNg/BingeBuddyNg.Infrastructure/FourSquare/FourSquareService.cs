using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.Infrastructure.FourSquare;
using Newtonsoft.Json;

namespace BingeBuddyNg.Infrastructure.FourSquare
{
    public class FourSquareService : IFourSquareService
    {
        private const string BaseUrl = "https://api.foursquare.com/v2";

        private readonly IHttpClientFactory httpClientFactory;
        private readonly FourSquareConfiguration configuration;

        public async Task<List<Venue>> SearchVenuesAsync(float latitude, float longitude)
        {
            var client = this.httpClientFactory.CreateClient();

            // force english format
            var englishCulture = CultureInfo.GetCultureInfo("en");
            string lalong = $"{latitude.ToString(englishCulture)},{longitude.ToString(englishCulture)}";
            string url = $"{BaseUrl}/venues/search?v=20190112&client_id={this.configuration.FourSquareApiClientKey}&client_secret={this.configuration.FourSquareApiClientSecret}&ll={lalong}";

            var response = await client.GetStringAsync(url);

            // convert
            var venueResult = JsonConvert.DeserializeObject<VenueRootObject>(response);

            return venueResult.Response.Venues;
        }
    }
}
