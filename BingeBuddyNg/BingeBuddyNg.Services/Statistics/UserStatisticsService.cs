using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Calculation;
using BingeBuddyNg.Services.Drink;
using BingeBuddyNg.Services.Statistics;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Statistics
{
    public class UserStatisticsService : IUserStatisticsService
    {
        public IActivityRepository ActivityRepository { get; }
        public IUserStatsRepository UserStatsRepository { get; }

        public ICalculationService CalculationService { get; }
        public IUserStatsHistoryRepository StatsHistoryRepository { get; }
        private ILogger<UserStatisticsService> logger;

        public UserStatisticsService(IActivityRepository activityRepository, IUserStatsRepository userStatsRepository, 
            IUserStatsHistoryRepository statsHistoryRepository,
            ICalculationService calculationService, ILogger<UserStatisticsService> logger)
        {
            this.ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.UserStatsRepository = userStatsRepository ?? throw new ArgumentNullException(nameof(userStatsRepository));
            this.StatsHistoryRepository = statsHistoryRepository ?? throw new ArgumentNullException(nameof(statsHistoryRepository));
            this.CalculationService = calculationService ?? throw new ArgumentNullException(nameof(calculationService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task UpdateRankingForUserAsync(string userId)
        {
            DateTime startTimestamp = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30));

            var drinkActivityLastMonth = await ActivityRepository.GetActivitysForUserAsync(userId, startTimestamp, ActivityType.Drink);

            // filter non-alcoholic drinks and calculate count
            var alcoholicDrinkCount = drinkActivityLastMonth.Count(d => d.DrinkType != DrinkType.Anti);

            await UserStatsRepository.UpdateTotalDrinkCountLastMonthAsync(userId, alcoholicDrinkCount);
        }
        public async Task<UserStatistics> UpdateStatsForUserAsync(User.User user)
        {
            var stats = await CalculationService.CalculateStatsForUserAsync(user);
            var userStats = new UserStatistics(user.Id, stats.CurrentAlcLevel, stats.CurrentNightDrinks);

            await UserStatsRepository.SaveStatisticsForUserAsync(userStats);

            try
            {
                if (userStats.CurrentAlcoholization > 0)
                {
                    await StatsHistoryRepository.SaveStatisticsHistoryAsync(new UserStatisticHistory(user.Id, DateTime.UtcNow, stats.CurrentAlcLevel));
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to save history entry for user [{user}].");
            }
            logger.LogDebug($"Successfully updated stats for user {user}: {userStats}");
            return userStats;
        }
    }
}
