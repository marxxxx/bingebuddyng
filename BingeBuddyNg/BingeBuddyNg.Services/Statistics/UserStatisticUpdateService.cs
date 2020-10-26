using System;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.Calculation;
using BingeBuddyNg.Core.Drink;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.User.Domain;
using Microsoft.Extensions.Logging;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.Statistics
{
    public class UserStatisticUpdateService
    {
        private readonly CalculationService calculationService;
        private readonly IActivityRepository activityRepository;
        private readonly IStorageAccessService storageAccessService;
        private readonly ILogger<UserStatisticUpdateService> logger;

        public UserStatisticUpdateService(IActivityRepository activityRepository, IStorageAccessService storageAccessService, ILogger<UserStatisticUpdateService> logger, CalculationService calculationService)
        {
            this.activityRepository = activityRepository;
            this.storageAccessService = storageAccessService;
            this.logger = logger;
            this.calculationService = calculationService;
        }

        public async Task<UserStatistics> UpdateStatisticsAsync(string userId, Gender gender, int? weight)
        {
            var stats = await calculationService.CalculateStatsForUserAsync(userId, gender, weight);
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

        public async Task UpdateRankingAsync(string userId)
        {
            DateTime startTimestamp = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30));

            var drinkActivityLastMonth = await activityRepository.GetUserActivitiesAsync(userId, startTimestamp, ActivityType.Drink);

            // filter non-alcoholic drinks and calculate count
            var alcoholicDrinkCount = drinkActivityLastMonth.Where(d => d.ActivityType == ActivityType.Drink).Count(d => d.DrinkType != DrinkType.Anti);

            await UpdateTotalDrinkCountLastMonthAsync(userId, alcoholicDrinkCount);
        }

        public async Task IncreaseScoreAsync(string userId, int additionalScore)
        {
            var entity = await storageAccessService.GetTableEntityAsync<UserStatsTableEntity>(TableNames.UserStats, StaticPartitionKeys.UserStats, userId);
            if (entity.Score == null)
            {
                entity.Score = 0;
            }
            entity.Score += additionalScore;

            await storageAccessService.ReplaceAsync(TableNames.UserStats, entity);
        }

        private async Task UpdateTotalDrinkCountLastMonthAsync(string userId, int count)
        {
            var entity = await storageAccessService.GetTableEntityAsync<UserStatsTableEntity>(TableNames.UserStats, StaticPartitionKeys.UserStats, userId);
            entity.TotalDrinksLastMonth = count;

            await storageAccessService.InsertOrReplaceAsync(TableNames.UserStats, entity);
        }
    }
}