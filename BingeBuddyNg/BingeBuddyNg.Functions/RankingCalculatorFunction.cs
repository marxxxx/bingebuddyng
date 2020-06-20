using System;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User.Queries;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BingeBuddyNg.Functions
{
    public class RankingCalculatorFunction
    {
        private readonly ISearchUsersQuery searchUsersQuery;
        private readonly IUserStatisticsService userStatisticsService;

        public RankingCalculatorFunction(ISearchUsersQuery searchUsersQuery, IUserStatisticsService userStatisticsService)
        {
            this.searchUsersQuery = searchUsersQuery ?? throw new ArgumentNullException(nameof(searchUsersQuery));
            this.userStatisticsService = userStatisticsService ?? throw new ArgumentNullException(nameof(userStatisticsService));
        }

        [FunctionName(nameof(RankingCalculatorFunction))]
        public async Task Run([TimerTrigger("0 0 */6 * * *")]TimerInfo myTimer, ILogger log)
        {
            var users = await searchUsersQuery.ExecuteAsync();

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
