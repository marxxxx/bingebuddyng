using System;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User.Queries;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BingeBuddyNg.Functions
{
    public class DrinkCalculatorFunction
    {
        private readonly ISearchUsersQuery getUsersQuery;
        private readonly IUserStatisticsService userStatisticsService;

        public DrinkCalculatorFunction(ISearchUsersQuery getUsersQuery,
            IUserStatisticsService userStatisticsService)
        {
            this.getUsersQuery = getUsersQuery ?? throw new ArgumentNullException(nameof(getUsersQuery));
            this.userStatisticsService = userStatisticsService ?? throw new ArgumentNullException(nameof(userStatisticsService));
        }

        [FunctionName(nameof(DrinkCalculatorFunction))]
        public async Task Run([TimerTrigger("0 */15 * * * *")]TimerInfo myTimer,
            ILogger log)
        {
            var users = await getUsersQuery.ExecuteAsync();

            foreach (var u in users)
            {
                try
                {
                    await userStatisticsService.UpdateStatsForUserAsync(u.Id, u.Gender, u.Weight);
                }
                catch (Exception ex)
                {
                    log.LogError($"Failed to update stats for user {u}: [{ex}]");
                }
            }
        }


    }
}
