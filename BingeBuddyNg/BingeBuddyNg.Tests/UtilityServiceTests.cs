using BingeBuddyNg.Services.Infrastructure;
using Moq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace BingeBuddyNg.Tests
{
    public class UtilityServiceTests
    {
        [Fact]
        public async Task Get_Location_From_Lat_Long_Returns_Complete_Result()
        {
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(m => m.CreateClient(It.IsAny<string>())).Returns(new HttpClient());

            var utilityService = new UtilityService(httpClientFactoryMock.Object, new GoogleApiConfiguration("key"));
            double latitude = 48.3202861;
            double longitude = 14.2917983;
            var address = await utilityService.GetAddressFromLongLatAsync(new Services.Activity.Location(latitude, longitude));
            Assert.NotNull(address);
            Assert.NotNull(address.AddressText);
            Assert.NotNull(address.CountryLongName);
            Assert.NotNull(address.CountryShortName);
        }
    }
}
