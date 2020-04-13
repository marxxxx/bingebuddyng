using System;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Calculation;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BingeBuddyNg.Functions
{
    public class RankingCalculatorFunction
    {
        public IUserRepository UserRepository { get; }
        
        public IUserStatisticsService UserStatisticsService { get; }

        public RankingCalculatorFunction(IUserRepository userRepository, IUserStatisticsService userStatisticsService)
        {
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.UserStatisticsService = userStatisticsService ?? throw new ArgumentNullException(nameof(userStatisticsService));
        }

        [FunctionName("RankingCalculatorFunction")]
        public async Task Run([TimerTrigger("0 0 */6 * * *")]TimerInfo myTimer, ILogger log)
        {
            var users = await UserRepository.GetUsersAsync();

            // Filter for active users
            var activeUsers = users.Where(u => u.LastOnline > DateTime.UtcNow.Subtract(TimeSpan.FromDays(30))).ToList();

            foreach (var u in activeUsers)
            {
                try
                {
                    log.LogInformation($"Calculating ranking for user [{u}] ...");
                    await UserStatisticsService.UpdateRankingForUserAsync(u.Id);
                }
                catch (Exception ex)
                {
                    log.LogError($"Failed to update ranking for user {u}: [{ex}]");
                }
            }
        }


    }
}
