using System;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.DrinkEvent.Persistence;
using BingeBuddyNg.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.DrinkEvent
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

        public async Task<Domain.DrinkEvent> CreateDrinkEventAsync(DateTime startTime, DateTime endTime)
        {
            var drinkEvent = new Domain.DrinkEvent(startTime, endTime);

            await this.storageAccessService.InsertAsync(TableNames.DrinkEvents, new JsonTableEntity<DrinkEventEntity>(StaticPartitionKeys.DrinkEvent, GetRowKey(endTime), drinkEvent.ToEntity()));

            return drinkEvent;
        }

        private string GetRowKey(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddHHmm");
        }

        public async Task<Domain.DrinkEvent> FindCurrentDrinkEventAsync()
        {
            string now = GetRowKey(DateTime.UtcNow);

            var queryResult = await storageAccessService.QueryTableAsync<JsonTableEntity<DrinkEventEntity>>(TableNames.DrinkEvents, StaticPartitionKeys.DrinkEvent, now, 1);

            var result = queryResult.ResultPage.FirstOrDefault()?.Entity;
            return new Domain.DrinkEvent(result.StartUtc, result.EndUtc, result.ScoringUserIds);
        }

        public async Task UpdateDrinkEventAsync(Domain.DrinkEvent drinkEvent)
        {
            string rowKey = GetRowKey(drinkEvent.EndUtc);
            
            var entity = await storageAccessService.GetTableEntityAsync<JsonTableEntity<DrinkEventEntity>>(TableNames.DrinkEvents, StaticPartitionKeys.DrinkEvent, rowKey);
            entity.Entity = drinkEvent.ToEntity();

            await storageAccessService.ReplaceAsync(TableNames.DrinkEvents, entity);
        }
    }
}
