using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Drink;
using BingeBuddyNg.Services.DrinkEvent;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BingeBuddyNg.Functions
{
    public class ActivityAddedFunction
    {
        private readonly IUtilityService utilityService;
        private readonly IActivityRepository activityRepository;
        private readonly IUserRepository userRepository;
        private readonly IUserStatsRepository userStatsRepository;
        private readonly INotificationService notificationService;
        private readonly IDrinkEventRepository drinkEventRepository;
        private readonly ITranslationService translationService;
        private readonly IUserStatisticsService userStatisticsService;
        
        private ILogger logger;
        private IDurableClient durableClient;

        public ActivityAddedFunction(IUtilityService utilityService, IActivityRepository activityRepository,
            IUserRepository userRepository, IUserStatsRepository userStatsRepository,
            INotificationService notificationService, IDrinkEventRepository drinkEventRepository,
            ITranslationService translationService, IUserStatisticsService userStatisticsService)
        {
            this.utilityService = utilityService ?? throw new ArgumentNullException(nameof(utilityService));
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.userStatsRepository = userStatsRepository ?? throw new ArgumentNullException(nameof(userStatsRepository));
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.drinkEventRepository = drinkEventRepository ?? throw new ArgumentNullException(nameof(drinkEventRepository));
            this.translationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
            this.userStatisticsService = userStatisticsService ?? throw new ArgumentNullException(nameof(userStatisticsService));
        }

        [FunctionName("ActivityAddedFunction")]
        public async Task Run(
            [EventGridTrigger]EventGridEvent gridEvent,
            [DurableClient]IDurableClient starter,
            ILogger log)
        {
            this.durableClient = starter ?? throw new ArgumentNullException(nameof(starter));
            this.logger = log;

            var activityAddedMessage = JsonConvert.DeserializeObject<ActivityAddedMessage>(gridEvent.Data.ToString());
            var activity = await activityRepository.GetActivityAsync(activityAddedMessage.ActivityId);

            log.LogInformation($"Handling added activity [{activity}] ...");


            try
            {
                User currentUser = null;
                try
                {
                    currentUser = await userRepository.FindUserAsync(activity.UserId);
                    if (activity.ActivityType == ActivityType.Drink)
                    {
                        await HandleMonitoringAsync(currentUser);
                    }
                }
                catch (Exception ex)
                {
                    log.LogError($"Error managing monitoring function", ex);
                }

                UserStatistics userStats = null;
                try
                {
                    // Immediately update Stats for current user
                    userStats = await userStatisticsService.UpdateStatsForUserAsync(currentUser);
                    activity.DrinkCount = userStats.CurrentNightDrinks;
                    await userStatisticsService.UpdateRankingForUserAsync(currentUser.Id);
                }
                catch (Exception ex)
                {
                    log.LogError($"Failed to update stats for user [{currentUser}]: [{ex}]");
                }

                bool shouldUpdate = false;

                if (activity.Location != null && activity.Location.IsValid())
                {
                    await HandleLocationUpdateAsync(activity);
                    shouldUpdate = true;
                }

                if (activity.ActivityType == ActivityType.Drink && userStats != null)
                {
                    activity.DrinkCount = userStats.CurrentNightDrinks;
                    activity.AlcLevel = userStats.CurrentAlcoholization;
                    shouldUpdate = true;
                }

                if (shouldUpdate)
                {
                    await activityRepository.UpdateActivityAsync(activity);
                }
                
                var friendUserIds = currentUser.GetVisibleFriendUserIds(true);
                // send Realtime-Notification via SignalR
                var activityStats = new ActivityStatsDTO(activity.ToDto(), userStats?.ToDto());
                await this.notificationService.SendSignalRMessageAsync(friendUserIds, Shared.Constants.SignalR.NotificationHubName, Shared.Constants.SignalR.ActivityReceivedMethodName, activityStats);

                // remind only first and every 5th drink this night to avoid spamming
                if (ShouldNotifyUsers(activity, userStats))
                {
                    // get friends of this user who didn't mute themselves from him
                    await HandleUserNotificationsAsync(friendUserIds.Where(u=> u != currentUser.Id).ToList().AsReadOnly(), activity);
                }

                // check for drink events
                try
                {
                    await HandleDrinkEventsAsync(activity, currentUser);
                }
                catch (Exception ex)
                {
                    log.LogError($"Error checking drink actions", ex);
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Processing failed: [{ex}]");
            }
        }

        private async Task HandleMonitoringAsync(User currentUser)
        {
            if (currentUser.MonitoringInstanceId != null)
            {
                await this.durableClient.TerminateAsync(currentUser.MonitoringInstanceId, "Drank early enough.");
            }

            // Start timer to remind user about entering his next drink.
            var monitoringInstanceId = await this.durableClient.StartNewAsync(nameof(DrinkReminderFunction), currentUser);
            await userRepository.UpdateMonitoringInstanceAsync(currentUser.Id, monitoringInstanceId);
        }

        private async Task HandleLocationUpdateAsync(Activity activity)
        {
            var address = await utilityService.GetAddressFromLongLatAsync(activity.Location);
            activity.LocationAddress = address.AddressText;
            activity.CountryLongName = address.CountryLongName;
            activity.CountryShortName = address.CountryShortName;
        }

        private async Task HandleUserNotificationsAsync(IReadOnlyList<string> friendUserIds, Activity activity)
        {            
            foreach (var friendUserId in friendUserIds)
            {
                try
                {
                    var friendUser = await userRepository.FindUserAsync(friendUserId);
                    if (friendUser != null && friendUser.PushInfo != null &&
                        friendUser.LastOnline > DateTime.UtcNow.Subtract(TimeSpan.FromDays(30)))
                    {
                        this.logger.LogInformation($"Sending push to [{friendUser}] ...");

                        var notificationMessage = await GetNotificationMessageAsync(friendUser.Language, activity);

                        notificationService.SendWebPushMessage(new[] { friendUser.PushInfo }, notificationMessage);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogError($"Failed to send push notification to user [{friendUserId}]: [{ex}]");
                }
            }
        }

        private async Task HandleDrinkEventsAsync(Activity activity, User currentUser)
        {
            if (activity.ActivityType == ActivityType.Drink || activity.DrinkType != DrinkType.Anti)
            {
                var drinkEvent = await drinkEventRepository.FindCurrentDrinkEventAsync();
                if (drinkEvent != null)
                {
                    if (drinkEvent.AddScoringUserId(currentUser.Id))
                    {
                        await drinkEventRepository.UpdateDrinkEventAsync(drinkEvent);

                        await userStatsRepository.IncreaseScoreAsync(currentUser.Id, Shared.Constants.Scores.StandardDrinkAction);

                        string message = await translationService.GetTranslationAsync(currentUser.Language, "DrinkEventActivityWinMessage", Shared.Constants.Scores.StandardDrinkAction);

                        var drinkEventNotificiationActivity = Activity.CreateNotificationActivity(DateTime.UtcNow, currentUser.Id, currentUser.Name, message);
                        await activityRepository.AddActivityAsync(drinkEventNotificiationActivity);

                        if (currentUser.PushInfo != null)
                        {
                            string notificationMessage = await translationService.GetTranslationAsync(currentUser.Language, "DrinkEventWinMessage", Shared.Constants.Scores.StandardDrinkAction);
                            notificationService.SendWebPushMessage(new[] { currentUser.PushInfo }, new NotificationMessage("Gratuliere!", notificationMessage));
                        }
                    }
                }
            }
        }

        private bool ShouldNotifyUsers(Activity activity, UserStatistics userStats)
        {
            bool shouldNotify = ((activity.ActivityType == ActivityType.Image || activity.ActivityType == ActivityType.Message) ||
                (userStats == null || userStats.CurrentNightDrinks == 1 || (userStats.CurrentNightDrinks % 5 == 0)));
            return shouldNotify;
        }

        private async Task<NotificationMessage> GetNotificationMessageAsync(string language, Activity activity)
        {
            string locationSnippet = null;
            if (!string.IsNullOrEmpty(activity.LocationAddress))
            {
                locationSnippet = await translationService.GetTranslationAsync(language, "DrinkLocationSnippet", activity.LocationAddress);
            }

            string activityString = null;
            switch (activity.ActivityType)
            {
                case ActivityType.Drink:
                    activityString = await translationService.GetTranslationAsync(language, "DrinkActivityMessage", activity.DrinkName, locationSnippet);
                    break;
                case ActivityType.Image:
                    activityString = await translationService.GetTranslationAsync(language, "ImageActivityMessage");
                    break;
                case ActivityType.Message:
                    activityString = await translationService.GetTranslationAsync(language, "MessageActivityMessage", activity.Message);
                    break;
                case ActivityType.VenueEntered:
                    activityString = await translationService.GetTranslationAsync(language, "VenueEnterActivityMessage", activity.Venue.Name);
                    break;
                case ActivityType.VenueLeft:
                    activityString = await translationService.GetTranslationAsync(language, "VenueLeaveActivityMessage", activity.Venue.Name);
                    break;
            }

            var notificationMessage = new NotificationMessage(Constants.NotificationIconUrl,
                Constants.NotificationIconUrl, Constants.ApplicationUrl, Constants.ApplicationName,
                    $"{activity.UserName} {activityString}");
            return notificationMessage;

        }
    }
}
