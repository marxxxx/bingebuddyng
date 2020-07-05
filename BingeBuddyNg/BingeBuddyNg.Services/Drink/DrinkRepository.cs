using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Drink.DTO;
using BingeBuddyNg.Core.Drink.Persistence;
using BingeBuddyNg.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace BingeBuddyNg.Core.Drink
{
    public class DrinkRepository : IDrinkRepository
    {
        private const string TableName = "drinks";

        private static readonly IEnumerable<DrinkDTO> DefaultDrinks = new List<DrinkDTO>()
            {
            new DrinkDTO("1", DrinkType.Beer, "Beer", 5, 500 ),
            new DrinkDTO("2", DrinkType.Wine, "Wine", 9, 125),
            new DrinkDTO("3", DrinkType.Shot, "Shot", 20, 40),
            new DrinkDTO("4", DrinkType.Anti, "Anti", 0, 250)
            };

        private readonly IStorageAccessService storageAccessService;

        private readonly ILogger<DrinkRepository> logger;

        public DrinkRepository(IStorageAccessService storageAccessService, ILogger<DrinkRepository> logger)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<DrinkDTO>> GetDrinksAsync(string userId)
        {
            var drinks = await this.storageAccessService.QueryTableAsync<DrinkTableEntity>(TableName, partitionKey: userId);

            if (drinks.ResultPage.Count == 0)
            {
                return DefaultDrinks;
            }
            else
            {
                return drinks.ResultPage.Select(d => d.ToDto()).ToList();
            }
        }

        public async Task<DrinkDTO> GetDrinkAsync(string userId, string drinkId)
        {
            var drink = await this.storageAccessService.GetTableEntityAsync<DrinkTableEntity>(TableName, userId, drinkId);
            return drink.ToDto();
        }

        public async Task SaveDrinksAsync(string userId, IEnumerable<DrinkDTO> drinks)
        {
            foreach (var d in drinks.Where(d => string.IsNullOrEmpty(d.Id)))
            {
                d.Id = Guid.NewGuid().ToString();
            }
            var entities = drinks.Select(d => d.ToEntity(userId));
            await this.storageAccessService.InsertOrReplaceAsync(TableName, entities);
        }

        public async Task CreateDefaultDrinksForUserAsync(string userId)
        {
            var defaultDrinksForUser = DefaultDrinks.Select(d => new DrinkDTO(Guid.NewGuid().ToString(), d.DrinkType, d.Name, d.AlcPrc, d.Volume));
            await SaveDrinksAsync(userId, defaultDrinksForUser);
        }

        public async Task DeleteDrinkAsync(string userId, string drinkId)
        {
            await this.storageAccessService.DeleteAsync(TableName, userId, drinkId);
        }
    }
}
