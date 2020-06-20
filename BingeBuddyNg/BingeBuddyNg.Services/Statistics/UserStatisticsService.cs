using System;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.Calculation;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Services.Drink;
using BingeBuddyNg.Services.Statistics;
using Microsoft.Extensions.Logging;

namespace BingeBuddyNg.Core.Statistics
{
    public class UserStatisticsService : IUserStatisticsService
    {
        private readonly IActivityRepository activityRepository;

        private readonly IUserStatsRepository userStatsRepository;

        private readonly ICalculationService calculationService;

        private readonly IUserStatsHistoryRepository statsHistoryRepository;

        private readonly ILogger<UserStatisticsService> logger;

        public UserStatisticsService(IActivityRepository activityRepository, IUserStatsRepository userStatsRepository,
            IUserStatsHistoryRepository statsHistoryRepository,
            ICalculationService calculationService, ILogger<UserStatisticsService> logger)
        {
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.userStatsRepository = userStatsRepository ?? throw new ArgumentNullException(nameof(userStatsRepository));
            this.statsHistoryRepository = statsHistoryRepository ?? throw new ArgumentNullException(nameof(statsHistoryRepository));
            this.calculationService = calculationService ?? throw new ArgumentNullException(nameof(calculationService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task UpdateRankingForUserAsync(string userId)
        {
            DateTime startTimestamp = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30));

            var drinkActivityLastMonth = await activityRepository.GetUserActivitiesAsync(userId, startTimestamp, ActivityType.Drink);

            // filter non-alcoholic drinks and calculate count
            var alcoholicDrinkCount = drinkActivityLastMonth.Where(d=>d.ActivityType == ActivityType.Drink).Count(d => d.DrinkType != DrinkType.Anti);

            await userStatsRepository.UpdateTotalDrinkCountLastMonthAsync(userId, alcoholicDrinkCount);
        }
        public async Task<UserStatistics> UpdateStatsForUserAsync(string userId, Gender gender, int? weight)
        {
            var stats = await calculationService.CalculateStatsForUserAsync(userId, gender, weight);
            var userStats = new UserStatistics(userId, stats.CurrentAlcLevel, stats.CurrentNightDrinks);

            await userStatsRepository.SaveStatisticsForUserAsync(userStats);

            try
            {
                if (userStats.CurrentAlcoholization > 0)
                {
                    await statsHistoryRepository.SaveStatisticsHistoryAsync(new UserStatisticHistory(userId, DateTime.UtcNow, stats.CurrentAlcLevel));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save history entry for user [{user}].");
            }
            logger.LogDebug($"Successfully updated stats for user {userId}: {userStats}");
            return userStats;
        }
    }
}
