using System;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BingeBuddyNg.Functions
{
    public static class DrinkEventFunction
    {
        public const int LuckyNumber = 5;

        public static readonly IUserRepository UserRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IUserRepository>();
        public static readonly IDrinkEventRepository DrinkEventRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IDrinkEventRepository>();
        public static readonly INotificationService NotificationService = ServiceProviderBuilder.Instance.Value.GetRequiredService<INotificationService>();

        [FunctionName("DrinkEventFunction")]
        public static async Task Run([TimerTrigger("0 */60 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Drink Event executed at: {DateTime.Now}");

            int max = 20;
            if(DateTime.UtcNow.DayOfWeek == DayOfWeek.Friday ||
                DateTime.UtcNow.DayOfWeek == DayOfWeek.Saturday)
            {

                max = 6;
            }

            int result = new Random().Next(max);

            log.LogInformation($"Random result was {result}.");

            if(result == LuckyNumber)
            {
                log.LogInformation($"Drink Event!!!!");

                await DrinkEventRepository.CreateDrinkEventAsync(DateTime.UtcNow, DateTime.UtcNow.AddMinutes(30));

                var users = await UserRepository.GetUsersAsync();
                foreach(var u in users)
                {
                    if (u.PushInfo != null)
                    {
                        var message = new NotificationMessage("!! Trinkaktion !!", $"Trink etwas innerhalb der nächsten halben Stunde und verdiene dir {Shared.Constants.Scores.StandardDrinkAction} Härtepunkte!");
                        NotificationService.SendMessage(new[] { u.PushInfo }, message);
                    }
                }
            }
        }
    }
}
