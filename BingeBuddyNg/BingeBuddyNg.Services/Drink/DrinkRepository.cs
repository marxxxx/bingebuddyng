using BingeBuddyNg.Services.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Drink
{
    public class DrinkRepository : IDrinkRepository
    {
        private const string TableName = "drinks";

        private static readonly IEnumerable<Drink> defaultDrinks = new List<Drink>()
            {
            new Drink("1", DrinkType.Beer, "Beer", 5, 500 ),
            new Drink("2", DrinkType.Wine, "Wine", 9, 125),
            new Drink("3", DrinkType.Shot, "Shot", 20, 40),
            new Drink("4", DrinkType.Anti, "Anti", 0, 250)
            };

        private readonly IStorageAccessService storageAccessService;

        private readonly ILogger<DrinkRepository> logger;

        public DrinkRepository(IStorageAccessService storageAccessService, ILogger<DrinkRepository> logger)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Drink>> GetDrinksAsync(string userId)
        {
            string whereClause =
                   TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId);

            var drinks = await this.storageAccessService.QueryTableAsync<DrinkTableEntity>(TableName, whereClause);

            if (drinks.Count == 0)
            {
                return defaultDrinks;
            }
            else
            {
                return drinks.Select(d => d.ToDrink());
            }
        }

        public async Task<Drink> GetDrinkAsync(string userId, string drinkId)
        {
            var drink = await this.storageAccessService.GetTableEntityAsync<DrinkTableEntity>(TableName, userId, drinkId);
            return drink.ToDrink();
        }

        public async Task SaveDrinksAsync(string userId, IEnumerable<Drink> drinks)
        {
            foreach (var d in drinks.Where(d => string.IsNullOrEmpty(d.Id)))
            {
                d.Id = Guid.NewGuid().ToString();
            }

            TableBatchOperation batch = new TableBatchOperation();
            foreach (var drink in drinks)
            {
                var entity = new DrinkTableEntity(userId, drink);
                batch.Add(TableOperation.InsertOrReplace(entity));
            }

            var table = storageAccessService.GetTableReference(TableName);

            await table.ExecuteBatchAsync(batch);
        }

        public async Task DeleteDrinkAsync(string userId, string drinkId)
        {
            var entity = await storageAccessService.GetTableEntityAsync<DrinkTableEntity>(TableName, userId, drinkId);

            var table = storageAccessService.GetTableReference(TableName);
            await table.ExecuteAsync(TableOperation.Delete(entity));
        }
    }
}
