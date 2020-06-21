using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Drink;
using BingeBuddyNg.Core.Drink.Persistence;
using BingeBuddyNg.Services.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using Xunit;

namespace BingeBuddyNg.Tests
{
    public class DrinksTests
    {
        [Fact]
        public async Task GetDrinksShouldReturnDefaultDrinksIfNoDrinksAvailable()
        {
            string userId = "userId";
            var storageAccessServiceMock = new Mock<IStorageAccessService>();
            storageAccessServiceMock
                .Setup(s => s.QueryTableAsync<DrinkTableEntity>("drinks", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TableContinuationToken>()))
                .ReturnsAsync(new PagedQueryResult<DrinkTableEntity>(new List<DrinkTableEntity>(), (string)null));
            
            var drinkRepository = new DrinkRepository(storageAccessServiceMock.Object, new Mock<ILogger<DrinkRepository>>().Object);

            var result = await drinkRepository.GetDrinksAsync(userId);

            Assert.Equal(4, result.Count());
        }
    }
}
