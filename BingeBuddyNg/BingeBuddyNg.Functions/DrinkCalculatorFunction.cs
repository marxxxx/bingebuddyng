using System;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Ranking;
using BingeBuddyNg.Core.User;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BingeBuddyNg.Functions
{
    public class DrinkCalculatorFunction
    {
        private readonly IUserRepository userRepository;
        private readonly UserStatisticUpdateService statisticUpdateService;

        public DrinkCalculatorFunction(IUserRepository userRepository, UserStatisticUpdateService updateStatisticsCommand)
        {
            this.userRepository = userRepository;
            this.statisticUpdateService = updateStatisticsCommand;
        }

        [FunctionName(nameof(DrinkCalculatorFunction))]
        public async Task Run([TimerTrigger("0 */15 * * * *")]TimerInfo myTimer, ILogger log)
        {
            var users = await userRepository.SearchUsersAsync();

            foreach (var u in users)
            {
                try
                {
                    await this.statisticUpdateService.UpdateStatisticsAsync(u.Id, u.Gender, u.Weight);
                }
                catch (Exception ex)
                {
                    log.LogError($"Failed to update stats for user {u}: [{ex}]");
                }
            }
        }
    }
}
