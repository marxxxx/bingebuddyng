using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Services.Drink;
using BingeBuddyNg.Services.Infrastructure;
using Microsoft.WindowsAzure.Storage.Table;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.Statistics.Commands
{
    public class UpdateRankingCommand
    {
        private readonly IActivityRepository activityRepository;

        private readonly IStorageAccessService storageAccessService;

        public UpdateRankingCommand(IActivityRepository activityRepository, IStorageAccessService storageAccessService)
        {
            this.activityRepository = activityRepository;
            this.storageAccessService = storageAccessService;
        }

        public async Task ExecuteAsync(string userId)
        {
            DateTime startTimestamp = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30));

            var drinkActivityLastMonth = await activityRepository.GetUserActivitiesAsync(userId, startTimestamp, ActivityType.Drink);

            // filter non-alcoholic drinks and calculate count
            var alcoholicDrinkCount = drinkActivityLastMonth.Where(d => d.ActivityType == ActivityType.Drink).Count(d => d.DrinkType != DrinkType.Anti);

            await UpdateTotalDrinkCountLastMonthAsync(userId, alcoholicDrinkCount);
        }

        private async Task UpdateTotalDrinkCountLastMonthAsync(string userId, int count)
        {
            var table = storageAccessService.GetTableReference(TableNames.UserStats);

            var result = await storageAccessService.GetTableEntityAsync<UserStatsTableEntity>(TableNames.UserStats, StaticPartitionKeys.UserStats, userId);
            result.TotalDrinksLastMonth = count;

            TableOperation saveOperation = TableOperation.InsertOrReplace(result);

            await table.ExecuteAsync(saveOperation);
        }

    }
}
