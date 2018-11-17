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
        [FunctionName("DrinkCalculatorFunction")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer,
            ILogger log)
        {
            // we stick with poor man's DI for now
            IUserRepository userRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IUserRepository>();
            ICalculationService calculationService = ServiceProviderBuilder.Instance.Value.GetRequiredService<ICalculationService>();
            IUserStatsRepository userStatsRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IUserStatsRepository>();

            var users = await userRepository.GetAllUsersAsync();

            foreach (var u in users)
            {
                try
                {
                    await UpdateStatsForUserAsync(u, calculationService, userStatsRepository, log);
                }
                catch (Exception ex)
                {
                    log.LogError($"Failed to update stats for user {u}: [{ex}]");
                }
            }
        }

        public static async Task UpdateStatsForUserAsync(User user, ICalculationService calculationService, IUserStatsRepository userStatsRepository, ILogger log)
        {
            var stats = await calculationService.CalculateStatsForUserAsync(user);
            var userStats = new UserStatistics(user.Id, stats.CurrentAlcLevel, stats.CurrentNightDrinks);
            await userStatsRepository.SaveStatisticsForUserAsync(userStats);

            log.LogDebug($"Successfully updated stats for user {user}: {userStats}");
        }
    }
}
