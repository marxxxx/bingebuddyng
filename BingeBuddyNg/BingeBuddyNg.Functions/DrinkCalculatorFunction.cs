using BingeBuddyNg.Functions.DependencyInjection;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BingeBuddyNg.Functions
{
    public static class DrinkCalculatorFunction
    {
        [FunctionName("DrinkCalculatorFunction")]
        public static async Task Run([TimerTrigger("0 */15 * * * *")]TimerInfo myTimer,
            [Inject]IUserRepository userRepository,
            [Inject]ICalculationService calculationService,
            [Inject]IUserStatsRepository userStatsRepository,
            ILogger log)
        {
            var users = await userRepository.GetAllUsersAsync();

            foreach (var u in users)
            {
                try
                {
                    await UpdateStatsForUserasync(u, calculationService, userStatsRepository);
                }
                catch (Exception ex)
                {
                    log.LogError($"Failed to update stats for user {u}: [{ex}]");
                }
            }
        }

        public static async Task UpdateStatsForUserasync(User user, ICalculationService calculationService, IUserStatsRepository userStatsRepository)
        {
            var stats = await calculationService.CalculateStatsForUserAsync(user);
            await userStatsRepository.SaveStatisticsForUserAsync(new UserStatistics(user.Id, stats.CurrentAlcLevel, stats.CurrentNightDrinks));
        }
    }
}
