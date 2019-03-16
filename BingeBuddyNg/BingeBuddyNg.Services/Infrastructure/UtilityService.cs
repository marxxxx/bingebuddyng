using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity;
using Newtonsoft.Json;

namespace BingeBuddyNg.Services.Infrastructure
{
    public class UtilityService : IUtilityService
    {
        /// <summary>
        /// Using static client for now until IHttpClientFactory issue with .NET Core 2.2 is resolved.
        /// </summary>
        private static HttpClient httpClient = new HttpClient();

        public IHttpClientFactory HttpClientFactory { get; }
        public AppConfiguration Configuration { get; }

        public UtilityService(IHttpClientFactory httpClientFactory, AppConfiguration configuration)
        {
            this.HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
                

        public async Task<AddressInfo> GetAddressFromLongLatAsync(Location location)
        {
            string lat = location.Latitude.ToString().Replace(',', '.');
            string lon = location.Longitude.ToString().Replace(',', '.');
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={lat},{lon}&sensor=true&key={Configuration.GoogleAPIKey}";

            //var httpClient = this.HttpClientFactory.CreateClient();
            var result = await httpClient.GetStringAsync(url);

            GoogleGeoCodeResponse response = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(result);

            AddressInfo addressInfo = new AddressInfo();
            addressInfo.AddressText = response.results[0].formatted_address;
            var countryEntrys = response.results.Where(r => r.address_components != null && r.types != null && r.types.Any(t => t == "country")
                && r.address_components.Any(c => c.types != null && c.types.Contains("country"))).ToArray();

            var countryEntry = countryEntrys.FirstOrDefault();
            if (countryEntry != null)
            {
                var countryComponent = countryEntry.address_components.FirstOrDefault();
                if (countryComponent != null)
                {
                    addressInfo.CountryLongName = countryComponent.long_name;
                    addressInfo.CountryShortName = countryComponent.short_name;
                }
            }

            return addressInfo;
        }

    }
}
