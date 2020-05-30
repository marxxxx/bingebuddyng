using System;
using System.Threading.Tasks;
using BingeBuddyNg.Services.DrinkEvent;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BingeBuddyNg.Functions
{
    public class DrinkEventFunction
    {
        public const int LuckyNumber = 1;

        public IUserRepository UserRepository { get; }
        public IDrinkEventRepository DrinkEventRepository { get; }
        public INotificationService NotificationService { get; }
        public ITranslationService TranslationService { get; }

        public DrinkEventFunction(IUserRepository userRepository, IDrinkEventRepository drinkEventRepository, 
            INotificationService notificationService, ITranslationService translationService)
        {
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.DrinkEventRepository = drinkEventRepository ?? throw new ArgumentNullException(nameof(drinkEventRepository));
            this.NotificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.TranslationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
        }

        [FunctionName(nameof(DrinkEventFunction))]
        public async Task Run([TimerTrigger("0 */60 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Drink Event executed at: {DateTime.Now}");

            int max = CalculateEventProbability();

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
                        var subject = await TranslationService.GetTranslationAsync(u.Language, "DrinkEvent");
                        var messageContent = await TranslationService.GetTranslationAsync(u.Language, "DrinkEventNotificationMessage", Shared.Constants.Scores.StandardDrinkAction);
                        var message = new WebPushNotificationMessage(subject, messageContent);
                        NotificationService.SendWebPushMessage(new[] { u.PushInfo }, message);
                    }
                }
            }
        }

        private int CalculateEventProbability()
        {
            int max = 30;
            if ((DateTime.UtcNow.DayOfWeek == DayOfWeek.Friday ||
                DateTime.UtcNow.DayOfWeek == DayOfWeek.Saturday) &&
                (DateTime.UtcNow.Hour > 16))
            {
                max = 7;
            };

            if((DateTime.UtcNow.Day == 31 && DateTime.UtcNow.Month == 12 ) ||
                (DateTime.UtcNow.Day == 1 && DateTime.UtcNow.Month == 1 && DateTime.UtcNow.Hour < 8))
            {
                max = 5;
            }

            return max;
        }
    }
}
