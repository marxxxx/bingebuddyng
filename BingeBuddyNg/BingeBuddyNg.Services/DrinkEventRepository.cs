using BingeBuddyNg.Services.Entitys;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services
{
    public class DrinkEventRepository : IDrinkEventRepository
    {
        private const string TableName = "drinkevents";
        private const string PartitionKeyValue = "drinkevent";

        private ILogger<DrinkEventRepository> logger;

        public StorageAccessService StorageAccessService { get; }

        
        public DrinkEventRepository(StorageAccessService storageAccessService, ILogger<DrinkEventRepository> logger)
        {
            this.StorageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<DrinkEvent> CreateDrinkEventAsync(DateTime startTime, DateTime endTime)
        {
            var table = StorageAccessService.GetTableReference(TableName);

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

            var queryResult = await StorageAccessService.QueryTableAsync<JsonTableEntity<DrinkEvent>>(TableName, whereClause);

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

            var queryResult = await StorageAccessService.QueryTableAsync<JsonTableEntity<DrinkEvent>>(TableName, whereClause);
            var entity = queryResult.First();
            entity.Entity = drinkEvent;

            var table = StorageAccessService.GetTableReference(TableName);

            await table.ExecuteAsync(TableOperation.Replace(entity));
        }
    }
}
