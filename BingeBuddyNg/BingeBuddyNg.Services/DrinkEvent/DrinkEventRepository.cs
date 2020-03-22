using System;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace BingeBuddyNg.Services.DrinkEvent
{
    public class DrinkEventRepository : IDrinkEventRepository
    {
        private const string TableName = "drinkevents";
        private const string PartitionKeyValue = "drinkevent";

        private readonly IStorageAccessService storageAccessService;
        private readonly ILogger<DrinkEventRepository> logger;

        public DrinkEventRepository(IStorageAccessService storageAccessService, ILogger<DrinkEventRepository> logger)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<DrinkEvent> CreateDrinkEventAsync(DateTime startTime, DateTime endTime)
        {
            var table = storageAccessService.GetTableReference(TableName);

            var drinkEvent = new DrinkEvent(startTime, endTime);

            var operation = TableOperation.Insert(new JsonTableEntity<DrinkEvent>(PartitionKeyValue, GetRowKey(endTime), drinkEvent));
            await table.ExecuteAsync(operation);

            return drinkEvent;
        }

        private string GetRowKey(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddHHmm");
        }

        public async Task<DrinkEvent> FindCurrentDrinkEventAsync()
        {
            string now = GetRowKey(DateTime.UtcNow);
            string whereClause = 
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PartitionKeyValue), 
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, now)
                    );

            var queryResult = await storageAccessService.QueryTableAsync<JsonTableEntity<DrinkEvent>>(TableName, whereClause);

            var result = queryResult?.FirstOrDefault()?.Entity;
            return result;
        }

        

        public async Task UpdateDrinkEventAsync(DrinkEvent drinkEvent)
        {
            string rowKey = GetRowKey(drinkEvent.EndUtc);
            string whereClause =
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PartitionKeyValue),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey)
                    );

            var queryResult = await storageAccessService.QueryTableAsync<JsonTableEntity<DrinkEvent>>(TableName, whereClause);
            var entity = queryResult.First();
            entity.Entity = drinkEvent;

            var table = storageAccessService.GetTableReference(TableName);

            await table.ExecuteAsync(TableOperation.Replace(entity));
        }
    }
}
