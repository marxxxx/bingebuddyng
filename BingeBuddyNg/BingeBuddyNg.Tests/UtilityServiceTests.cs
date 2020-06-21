using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Infrastructure;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace BingeBuddyNg.Tests
{
    public class UtilityServiceTests
    {
        public async Task Get_Location_From_Lat_Long_Returns_Complete_Result()
        {
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(m => m.CreateClient(It.IsAny<string>())).Returns(new HttpClient());

            var utilityService = new AddressDecodingService(httpClientFactoryMock.Object, new GoogleApiConfiguration("key"));
            double latitude = 48.3202861;
            double longitude = 14.2917983;
            var address = await utilityService.GetAddressFromLongLatAsync(new Location(latitude, longitude));
            Assert.NotNull(address);
            Assert.NotNull(address.AddressText);
            Assert.NotNull(address.CountryLongName);
            Assert.NotNull(address.CountryShortName);
        }

        public void DeserializeInCamelCase()
        {
            var activity = new ActivityDTO();
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var str = JsonConvert.SerializeObject(activity, new JsonSerializerSettings() { ContractResolver = contractResolver } );
            Assert.Contains(str, "id");
        }
    }
}
