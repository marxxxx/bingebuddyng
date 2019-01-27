using System;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Calculation;
using BingeBuddyNg.Services.User;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BingeBuddyNg.Functions
{
    public static class RankingCalculatorFunction
    {
        // we stick with poor man's DI for now
        public static readonly IUserRepository UserRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IUserRepository>();
        public static readonly IActivityRepository ActivityRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IActivityRepository>();
        public static readonly IUserStatsRepository UserStatsRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IUserStatsRepository>();

        [FunctionName("RankingCalculatorFunction")]
        public static async Task Run([TimerTrigger("0 0 */6 * * *")]TimerInfo myTimer, ILogger log)
        {

            var users = await UserRepository.GetUsersAsync();

            foreach (var u in users)
            {
                try
                {
                    log.LogInformation($"Calculating ranking for user [{u}] ...");
                    await UpdateRankingForUserAsync(u.Id, log);
                }
                catch (Exception ex)
                {
                    log.LogError($"Failed to update ranking for user {u}: [{ex}]");
                }
            }
        }

        public static async Task UpdateRankingForUserAsync(string userId, ILogger log)
        {
            DateTime startTimestamp = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30));

            var drinkActivityLastMonth = await ActivityRepository.GetActivitysForUserAsync(userId, startTimestamp, ActivityType.Drink);
            
            // filter non-alcoholic drinks and calculate count
            var alcoholicDrinkCount = drinkActivityLastMonth.Count(d => d.DrinkType != DrinkType.Anti);

            await UserStatsRepository.UpdateTotalDrinkCountLastMonthAsync(userId, alcoholicDrinkCount);
        }
    }
}
