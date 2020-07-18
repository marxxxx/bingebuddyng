using System;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Statistics.Commands;
using BingeBuddyNg.Core.User.Queries;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BingeBuddyNg.Functions
{
    public class DrinkCalculatorFunction
    {
        private readonly SearchUsersQuery getUsersQuery;
        private readonly UpdateStatisticsCommand updateStatisticsCommand;

        public DrinkCalculatorFunction(SearchUsersQuery getUsersQuery, UpdateStatisticsCommand updateStatisticsCommand)
        {
            this.getUsersQuery = getUsersQuery;
            this.updateStatisticsCommand = updateStatisticsCommand;
        }

        [FunctionName(nameof(DrinkCalculatorFunction))]
        public async Task Run([TimerTrigger("0 */15 * * * *")]TimerInfo myTimer, ILogger log)
        {
            var users = await getUsersQuery.ExecuteAsync();

            foreach (var u in users)
            {
                try
                {
                    await updateStatisticsCommand.ExecuteAsync(u.Id, u.Gender, u.Weight);
                }
                catch (Exception ex)
                {
                    log.LogError($"Failed to update stats for user {u}: [{ex}]");
                }
            }
        }
    }
}
