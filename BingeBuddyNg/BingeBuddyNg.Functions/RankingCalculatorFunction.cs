using System;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Statistics;
using BingeBuddyNg.Core.User;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BingeBuddyNg.Functions
{
    public class RankingCalculatorFunction
    {
        private readonly IUserRepository userRepository;
        private readonly UserStatisticUpdateService rankingService;

        public RankingCalculatorFunction(IUserRepository userRepository, UserStatisticUpdateService rankingService)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.rankingService = rankingService?? throw new ArgumentNullException(nameof(rankingService));
        }

        [FunctionName(nameof(RankingCalculatorFunction))]
        public async Task Run([TimerTrigger("0 0 */6 * * *")]TimerInfo myTimer, ILogger log)
        {
            var users = await userRepository.SearchUsersAsync();

            // Filter for active users
            var activeUsers = users.Where(u => u.LastOnline > DateTime.UtcNow.Subtract(TimeSpan.FromDays(30))).ToList();

            foreach (var u in activeUsers)
            {
                try
                {
                    log.LogInformation($"Calculating ranking for user [{u}] ...");
                    await rankingService.UpdateRankingAsync(u.Id);
                }
                catch (Exception ex)
                {
                    log.LogError($"Failed to update ranking for user {u}: [{ex}]");
                }
            }
        }
    }
}
