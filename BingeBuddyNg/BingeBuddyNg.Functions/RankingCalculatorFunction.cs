using System;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BingeBuddyNg.Functions
{
    public class RankingCalculatorFunction
    {
        private readonly IUserRepository userRepository;
        private readonly IUserStatisticsService userStatisticsService;

        public RankingCalculatorFunction(IUserRepository userRepository, IUserStatisticsService userStatisticsService)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.userStatisticsService = userStatisticsService ?? throw new ArgumentNullException(nameof(userStatisticsService));
        }

        [FunctionName(nameof(RankingCalculatorFunction))]
        public async Task Run([TimerTrigger("0 0 */6 * * *")]TimerInfo myTimer, ILogger log)
        {
            var users = await userRepository.GetUsersAsync();

            // Filter for active users
            var activeUsers = users.Where(u => u.LastOnline > DateTime.UtcNow.Subtract(TimeSpan.FromDays(30))).ToList();

            foreach (var u in activeUsers)
            {
                try
                {
                    log.LogInformation($"Calculating ranking for user [{u}] ...");
                    await userStatisticsService.UpdateRankingForUserAsync(u.Id);
                }
                catch (Exception ex)
                {
                    log.LogError($"Failed to update ranking for user {u}: [{ex}]");
                }
            }
        }
    }
}
