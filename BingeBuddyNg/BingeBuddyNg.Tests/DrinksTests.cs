using BingeBuddyNg.Services.Drink;
using BingeBuddyNg.Services.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            storageAccessServiceMock.Setup(s => s.QueryTableAsync<DrinkTableEntity>("drinks", It.IsAny<string>())).ReturnsAsync(
                new List<DrinkTableEntity>());
            
            var drinkRepository = new DrinkRepository(storageAccessServiceMock.Object, new Mock<ILogger<DrinkRepository>>().Object);

            var result = await drinkRepository.GetDrinksAsync(userId);

            Assert.Equal(4, result.Count());
        }
    }
}
