using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Activity.Domain;
using BingeBuddyNg.Services.Calculation;
using BingeBuddyNg.Services.Drink;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Statistics
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
            var alcoholicDrinkCount = drinkActivityLastMonth.OfType<DrinkActivity>().Count(d => d.DrinkType != DrinkType.Anti);

            await userStatsRepository.UpdateTotalDrinkCountLastMonthAsync(userId, alcoholicDrinkCount);
        }
        public async Task<UserStatistics> UpdateStatsForUserAsync(User.User user)
        {
            var stats = await calculationService.CalculateStatsForUserAsync(user);
            var userStats = new UserStatistics(user.Id, stats.CurrentAlcLevel, stats.CurrentNightDrinks);

            await userStatsRepository.SaveStatisticsForUserAsync(userStats);

            try
            {
                if (userStats.CurrentAlcoholization > 0)
                {
                    await statsHistoryRepository.SaveStatisticsHistoryAsync(new UserStatisticHistory(user.Id, DateTime.UtcNow, stats.CurrentAlcLevel));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save history entry for user [{user}].");
            }
            logger.LogDebug($"Successfully updated stats for user {user}: {userStats}");
            return userStats;
        }
    }
}
