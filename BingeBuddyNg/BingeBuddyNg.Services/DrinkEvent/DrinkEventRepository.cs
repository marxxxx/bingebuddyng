using System;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;
using Microsoft.Extensions.Logging;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Services.DrinkEvent
{
    public class DrinkEventRepository : IDrinkEventRepository
    {
        private readonly IStorageAccessService storageAccessService;
        private readonly ILogger<DrinkEventRepository> logger;

        public DrinkEventRepository(IStorageAccessService storageAccessService, ILogger<DrinkEventRepository> logger)
        {
            this.storageAccessService = storageAccessService;
            this.logger = logger;
        }

        public async Task<DrinkEvent> CreateDrinkEventAsync(DateTime startTime, DateTime endTime)
        {
            var drinkEvent = new DrinkEvent(startTime, endTime);

            await this.storageAccessService.InsertAsync(TableNames.DrinkEvents, new JsonTableEntity<DrinkEvent>(StaticPartitionKeys.DrinkEvent, GetRowKey(endTime), drinkEvent));

            return drinkEvent;
        }

        private string GetRowKey(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddHHmm");
        }

        public async Task<DrinkEvent> FindCurrentDrinkEventAsync()
        {
            string now = GetRowKey(DateTime.UtcNow);

            var queryResult = await storageAccessService.QueryTableAsync<JsonTableEntity<DrinkEvent>>(TableNames.DrinkEvents, StaticPartitionKeys.DrinkEvent, now, 1);

            var result = queryResult.ResultPage.FirstOrDefault()?.Entity;
            return result;
        }

        public async Task UpdateDrinkEventAsync(DrinkEvent drinkEvent)
        {
            string rowKey = GetRowKey(drinkEvent.EndUtc);
            
            var entity = await storageAccessService.GetTableEntityAsync<JsonTableEntity<DrinkEvent>>(TableNames.DrinkEvents, StaticPartitionKeys.DrinkEvent, rowKey);
            entity.Entity = drinkEvent;

            var table = storageAccessService.ReplaceAsync(TableNames.DrinkEvents, entity);
        }
    }
}
