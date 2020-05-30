using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Functions.Services.Notifications;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Drink;
using BingeBuddyNg.Services.DrinkEvent;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Shared;

namespace BingeBuddyNg.Functions.Services
{
    public class DrinkEventHandlingService
    {
        private readonly IDrinkEventRepository drinkEventRepository;
        private readonly IUserStatsRepository userStatsRepository;
        private readonly ITranslationService translationService;
        private readonly IActivityRepository activityRepository;
        
        public DrinkEventHandlingService(
            IDrinkEventRepository drinkEventRepository, 
            IUserStatsRepository userStatsRepository, 
            ITranslationService translationService, 
            IActivityRepository activityRepository)
        {
            this.drinkEventRepository = drinkEventRepository ?? throw new ArgumentNullException(nameof(drinkEventRepository));
            this.userStatsRepository = userStatsRepository ?? throw new ArgumentNullException(nameof(userStatsRepository));
            this.translationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        public async Task<IEnumerable<NotificationBase>> HandleDrinkEventsAsync(Activity activity, User currentUser)
        {
            if (activity.ActivityType != ActivityType.Drink && activity.DrinkType == DrinkType.Anti)
            {
                return Enumerable.Empty<NotificationBase>();
            }
            var drinkEvent = await drinkEventRepository.FindCurrentDrinkEventAsync();
            if (drinkEvent == null)
            {
                return Enumerable.Empty<NotificationBase>();
            }

            if (!drinkEvent.AddScoringUserId(currentUser.Id))
            {
                return Enumerable.Empty<NotificationBase>();
            }

            await drinkEventRepository.UpdateDrinkEventAsync(drinkEvent);

            await userStatsRepository.IncreaseScoreAsync(currentUser.Id, Constants.Scores.StandardDrinkAction);

            string message = await translationService.GetTranslationAsync(currentUser.Language, "DrinkEventActivityWinMessage", Constants.Scores.StandardDrinkAction);

            var drinkEventNotificationActivity = Activity.CreateNotificationActivity(DateTime.UtcNow, currentUser.Id, currentUser.Name, message);
            await activityRepository.AddActivityAsync(drinkEventNotificationActivity);

            return new[] { new DrinkEventCongratulationNotification(currentUser.Id) };
        }
    }
}
