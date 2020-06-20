using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Services.Infrastructure.Generated;
using Newtonsoft.Json;

namespace BingeBuddyNg.Services.Infrastructure
{
    public class UtilityService : IUtilityService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly GoogleApiConfiguration configuration;

        public UtilityService(IHttpClientFactory httpClientFactory, GoogleApiConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<AddressInfo> GetAddressFromLongLatAsync(Location location)
        {
            string lat = location.Latitude.ToString().Replace(',', '.');
            string lon = location.Longitude.ToString().Replace(',', '.');
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={lat},{lon}&sensor=true&key={configuration.ApiKey}";

            var httpClient = this.httpClientFactory.CreateClient();
            var result = await httpClient.GetStringAsync(url);

            var response = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(result);

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
