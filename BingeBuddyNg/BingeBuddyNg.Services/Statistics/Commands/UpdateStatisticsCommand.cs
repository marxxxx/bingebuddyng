using System;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Calculation;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Statistics;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.Statistics.Commands
{
    public class UpdateStatisticsCommand
    {
        private readonly CalculationService calculationService;
        private readonly IStorageAccessService storageAccessService;
        private readonly ILogger<UpdateStatisticsCommand> logger;

        public UpdateStatisticsCommand(
            CalculationService calculationService,
            IStorageAccessService storageAccessService, 
            ILogger<UpdateStatisticsCommand> logger)
        {
            this.calculationService = calculationService;
            this.storageAccessService = storageAccessService;
            this.logger = logger;
        }

        public async Task<UserStatistics> ExecuteAsync(string userId, Gender gender, int? weight)
        {
            var stats = await this.calculationService.CalculateStatsForUserAsync(userId, gender, weight);
            var userStats = new UserStatistics(userId, stats.CurrentAlcLevel, stats.CurrentNightDrinks);

            await SaveStatisticsForUserAsync(userStats);

            try
            {
                if (userStats.CurrentAlcoholization > 0)
                {
                    await AddStatisticHistoryAsync(new UserStatisticHistory(userId, DateTime.UtcNow, stats.CurrentAlcLevel));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save history entry for user [{user}].");
            }
            logger.LogDebug($"Successfully updated stats for user {userId}: {userStats}");
            return userStats;
        }

        private async Task AddStatisticHistoryAsync(UserStatisticHistory userStatistics)
        {
            var entity = new UserStatisticHistoryTableEntity(userStatistics.UserId, DateTime.UtcNow,
                userStatistics.CurrentAlcLevel);

            await storageAccessService.InsertAsync(TableNames.UserStatsHistory, entity);
        }

        private async Task SaveStatisticsForUserAsync(UserStatistics userStatistics)
        {
            var entity = new UserStatsTableEntity(StaticPartitionKeys.UserStats, userStatistics.UserId,
                userStatistics.CurrentAlcoholization, userStatistics.CurrentNightDrinks,
                userStatistics.Score,
                userStatistics.TotalDrinksLastMonth);

            await storageAccessService.InsertOrMergeAsync(TableNames.UserStats, entity);
        }
    }
}
