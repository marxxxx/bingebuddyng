using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BingeBuddyNg.Functions
{
    public static class DrinkCalculatorFunction
    {
        // we stick with poor man's DI for now
        public static readonly IUserRepository UserRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IUserRepository>();
        public static readonly ICalculationService CalculationService = ServiceProviderBuilder.Instance.Value.GetRequiredService<ICalculationService>();
        public static readonly IUserStatsRepository UserStatsRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IUserStatsRepository>();

        [FunctionName("DrinkCalculatorFunction")]
        public static async Task Run([TimerTrigger("0 */15 * * * *")]TimerInfo myTimer,
            ILogger log)
        {
            var users = await UserRepository.GetUsersAsync();

            foreach (var u in users)
            {
                try
                {
                    await UpdateStatsForUserAsync(u, log);
                }
                catch (Exception ex)
                {
                    log.LogError($"Failed to update stats for user {u}: [{ex}]");
                }
            }
        }

        public static async Task<UserStatistics> UpdateStatsForUserAsync(User user, ILogger log)
        {
            var stats = await CalculationService.CalculateStatsForUserAsync(user);
            var userStats = new UserStatistics(user.Id, stats.CurrentAlcLevel, stats.CurrentNightDrinks);
            await UserStatsRepository.SaveStatisticsForUserAsync(userStats);

            log.LogDebug($"Successfully updated stats for user {user}: {userStats}");
            return userStats;
        }
    }
}
