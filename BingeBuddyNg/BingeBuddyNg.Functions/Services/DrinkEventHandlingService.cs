using System;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.Drink;
using BingeBuddyNg.Core.DrinkEvent;
using BingeBuddyNg.Core.Statistics.Commands;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Core.User.Domain;
using BingeBuddyNg.Functions.Services.Notifications;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User.Queries;
using BingeBuddyNg.Shared;
using Microsoft.Extensions.Logging;

namespace BingeBuddyNg.Functions.Services
{
    public class DrinkEventHandlingService
    {
        private const int LuckyNumber = 1;

        private readonly SearchUsersQuery getUsersQuery;
        private readonly IDrinkEventRepository drinkEventRepository;
        private readonly IncreaseScoreCommand increaseScoreCommand;
        private readonly ITranslationService translationService;
        private readonly IActivityRepository activityRepository;
        private readonly PushNotificationService pushNotificationService;
        private readonly ILogger<DrinkEventHandlingService> logger;

        public DrinkEventHandlingService(
            SearchUsersQuery getUsersQuery,
            IDrinkEventRepository drinkEventRepository,
            IncreaseScoreCommand increaseScoreCommand, 
            ITranslationService translationService, 
            IActivityRepository activityRepository,
            PushNotificationService pushNotificationService,
            ILogger<DrinkEventHandlingService> logger)
        {
            this.getUsersQuery = getUsersQuery ?? throw new ArgumentNullException(nameof(getUsersQuery));
            this.drinkEventRepository = drinkEventRepository ?? throw new ArgumentNullException(nameof(drinkEventRepository));
            this.increaseScoreCommand = increaseScoreCommand ?? throw new ArgumentNullException(nameof(increaseScoreCommand));
            this.translationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.pushNotificationService = pushNotificationService ?? throw new ArgumentNullException(nameof(pushNotificationService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleDrinkEventsAsync(Activity activity, User currentUser)
        {
            if (activity.Drink.DrinkType == DrinkType.Anti)
            {
                return;
            }
            var drinkEvent = await drinkEventRepository.FindCurrentDrinkEventAsync();
            if (drinkEvent == null)
            {
                return;
            }

            if (!drinkEvent.AddScoringUserId(currentUser.Id))
            {
                return;
            }

            await drinkEventRepository.UpdateDrinkEventAsync(drinkEvent);

            await increaseScoreCommand.ExecuteAsync(currentUser.Id, Constants.Scores.StandardDrinkAction);

            string message = await translationService.GetTranslationAsync(currentUser.Language, "DrinkEventActivityWinMessage", Constants.Scores.StandardDrinkAction);

            var drinkEventNotificationActivity = Activity.CreateNotificationActivity(currentUser.Id, currentUser.Name, message);
            await activityRepository.AddActivityAsync(drinkEventNotificationActivity.ToEntity());

            var notifications = new[] { new DrinkEventCongratulationNotification(currentUser.Id) };
            await pushNotificationService.NotifyAsync(notifications);
        }

        public async Task TryCreateDrinkEventAsync()
        {
            int max = CalculateEventProbability();

            int result = new Random().Next(max);

            logger.LogInformation($"Random result was {result}.");

            if (result != LuckyNumber)
            {
                return;
            }

            logger.LogInformation($"Drink Event!!!!");

            await drinkEventRepository.CreateDrinkEventAsync(DateTime.UtcNow, DateTime.UtcNow.AddMinutes(30));

            var users = await getUsersQuery.ExecuteAsync();

            var notifications = users.Select(u => new DrinkEventNotification(u.Id));
            await this.pushNotificationService.NotifyAsync(notifications);            
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

            if ((DateTime.UtcNow.Day == 31 && DateTime.UtcNow.Month == 12) ||
                (DateTime.UtcNow.Day == 1 && DateTime.UtcNow.Month == 1 && DateTime.UtcNow.Hour < 8))
            {
                max = 5;
            }

            return max;
        }
    }
}
