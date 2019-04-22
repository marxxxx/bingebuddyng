using BingeBuddyNg.Services.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Drink
{
    public class DrinkRepository : IDrinkRepository
    {
        private const string TableName = "drinks";


        private static readonly IEnumerable<Drink> DefaultDrinks = new List<Drink>()
            {
            new Drink("1", DrinkType.Beer, "Beer", 5, 500 ),
            new Drink("2", DrinkType.Wine, "Wine", 9, 125),
            new Drink("3", DrinkType.Shot, "Shot", 20, 40),
            new Drink("4", DrinkType.Anti, "Anti", 0, 250)
            };

        public StorageAccessService StorageAccessService { get; }

        private ILogger<DrinkRepository> logger;

        public DrinkRepository(StorageAccessService storageAccessService, ILogger<DrinkRepository> logger)
        {
            this.StorageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Drink>> GetDrinksAsync(string userId)
        {
            string whereClause =
                   TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId);

            var drinks = await this.StorageAccessService.QueryTableAsync<DrinkEntity>(TableName, whereClause);

            if(drinks.Count == 0)
            {
                return DefaultDrinks;
            }
            else
            {
                return drinks.Select(d=>d.ToDrink());
            }
        }

        public async Task SaveDrinksAsync(string userId, IEnumerable<Drink> drinks)
        {
            TableBatchOperation batch = new TableBatchOperation();
            foreach(var drink in drinks)
            {
                var entity = new DrinkEntity(userId, drink);
                batch.Add(TableOperation.InsertOrReplace(entity));
            }

            var table = StorageAccessService.GetTableReference(TableName);

            await table.ExecuteBatchAsync(batch);
        }

        public async Task DeleteDrinkAsync(string userId, string drinkId)
        {
            var entity = await StorageAccessService.GetTableEntityAsync<DrinkEntity>(TableName, userId, drinkId);

            var table = StorageAccessService.GetTableReference(TableName);
            await table.ExecuteAsync(TableOperation.Delete(entity));
        }

    }
}
