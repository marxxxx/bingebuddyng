using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Calculation;
using BingeBuddyNg.Services.User;

namespace BingeBuddyNg.Functions
{
    public class DrinkCalculatorFunction
    {
        public IUserRepository UserRepository { get; }
        public IUserStatisticsService UserStatisticsService { get; }

        public DrinkCalculatorFunction(IUserRepository userRepository,
            IUserStatisticsService userStatisticsService)
        {
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.UserStatisticsService = userStatisticsService ?? throw new ArgumentNullException(nameof(userStatisticsService));
        }

        [FunctionName("DrinkCalculatorFunction")]
        public async Task Run([TimerTrigger("0 */15 * * * *")]TimerInfo myTimer,
            ILogger log)
        {
            var users = await UserRepository.GetUsersAsync();

            foreach (var u in users)
            {
                try
                {
                    await UserStatisticsService.UpdateStatsForUserAsync(u);
                }
                catch (Exception ex)
                {
                    log.LogError($"Failed to update stats for user {u}: [{ex}]");
                }
            }
        }

        
    }
}
