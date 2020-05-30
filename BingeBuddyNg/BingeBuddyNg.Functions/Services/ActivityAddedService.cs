using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Functions.Services.Notifications;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Shared;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace BingeBuddyNg.Functions.Services
{
    public class ActivityAddedService
    {
        private readonly IUtilityService utilityService;
        private readonly IActivityRepository activityRepository;
        private readonly IUserRepository userRepository;
        private readonly INotificationService notificationService;
        private readonly IUserStatisticsService userStatisticsService;
        private readonly ActivityDistributionService activityDistributionService;
        private readonly DrinkEventHandlingService drinkEventHandlingService;
        private readonly PushNotificationService pushNotificationService;
        private readonly ILogger<ActivityAddedService> logger;

        public ActivityAddedService(
            IUtilityService utilityService,
            IActivityRepository activityRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IUserStatisticsService userStatisticsService,
            ActivityDistributionService activityDistributionService,
            DrinkEventHandlingService drinkEventHandlingService,
            PushNotificationService pushNotificationService,
            ILogger<ActivityAddedService> logger)
        {
            this.utilityService = utilityService ?? throw new ArgumentNullException(nameof(utilityService));
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.userStatisticsService = userStatisticsService ?? throw new ArgumentNullException(nameof(userStatisticsService));
            this.activityDistributionService = activityDistributionService ?? throw new ArgumentNullException(nameof(activityDistributionService));
            this.drinkEventHandlingService = drinkEventHandlingService ?? throw new ArgumentNullException(nameof(drinkEventHandlingService));
            this.pushNotificationService = pushNotificationService ?? throw new ArgumentNullException(nameof(pushNotificationService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RunAsync(ActivityAddedMessage message, IDurableOrchestrationClient durableClient)
        {
            var activity = await activityRepository.GetActivityAsync(message.ActivityId);

            logger.LogInformation($"Handling added activity [{activity}] ...");

            var currentUser = await userRepository.FindUserAsync(activity.UserId);
            if (currentUser == null)
            {
                logger.LogError($"User [{activity.UserId}] not found!");
                return;
            }

            try
            {                
                if (activity.ActivityType == ActivityType.Drink)
                {
                    await HandleMonitoringAsync(durableClient, currentUser);
                }

                UserStatistics userStats = null;
                bool shouldUpdate = false;

                try
                {
                    if (activity.ActivityType == ActivityType.Drink && userStats != null)
                    {
                        // Immediately update Stats for current user
                        userStats = await userStatisticsService.UpdateStatsForUserAsync(currentUser);
                        activity.DrinkCount = userStats.CurrentNightDrinks;
                        await userStatisticsService.UpdateRankingForUserAsync(currentUser.Id);

                        activity.DrinkCount = userStats.CurrentNightDrinks;
                        activity.AlcLevel = userStats.CurrentAlcoholization;

                        shouldUpdate = true;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Failed to update stats for user [{currentUser}]");
                }

                if (activity.Location != null && activity.Location.IsValid())
                {
                    await HandleLocationUpdateAsync(activity);
                    shouldUpdate = true;
                }

                if (shouldUpdate)
                {
                    await activityRepository.UpdateActivityAsync(activity);
                }

                await this.activityDistributionService.DistributeActivitiesAsync(currentUser, activity);

                await SendActivityUpdateAsync(currentUser, activity, userStats);

                // check for drink events
                try
                {
                    await this.drinkEventHandlingService.HandleDrinkEventsAsync(activity, currentUser);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error handling drink events!");
                }

                // send out push notifications
                var notifications = BuildNotifications(currentUser, activity, userStats);
                if (notifications.Any())
                {
                    await pushNotificationService.NotifyAsync(notifications);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Processing failed unexpectedly!");
            }
        }

        private async Task HandleMonitoringAsync(IDurableOrchestrationClient durableClient, User currentUser)
        {
            try
            {
                if (currentUser.MonitoringInstanceId != null)
                {
                    await durableClient.TerminateAsync(currentUser.MonitoringInstanceId, "Drank early enough.");
                }

                // Start timer to remind user about entering his next drink.
                var monitoringInstanceId = await durableClient.StartNewAsync(nameof(DrinkReminderFunction), currentUser);
                await userRepository.UpdateMonitoringInstanceAsync(currentUser.Id, monitoringInstanceId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error managing monitoring function");
            }
        }

        private async Task HandleLocationUpdateAsync(Activity activity)
        {
            var address = await utilityService.GetAddressFromLongLatAsync(activity.Location);
            activity.LocationAddress = address.AddressText;
            activity.CountryLongName = address.CountryLongName;
            activity.CountryShortName = address.CountryShortName;
        }

        private async Task SendActivityUpdateAsync(User currentUser, Activity activity, UserStatistics userStats)
        {
            var friendsAndMeUserIds = currentUser.GetVisibleFriendUserIds(true);

            var activityStats = new ActivityStatsDTO(activity.ToDto(), userStats?.ToDto());

            // send Realtime-Notification via SignalR
            await this.notificationService.SendSignalRMessageAsync(
                friendsAndMeUserIds,
                Constants.SignalR.NotificationHubName,
                Constants.SignalR.ActivityReceivedMethodName,
                activityStats);
        }

        private List<NotificationBase> BuildNotifications(User currentUser, Activity activity, UserStatistics userStats)
        {
            var friendsUserIds = currentUser.GetVisibleFriendUserIds(false);

            List<NotificationBase> notifications = new List<NotificationBase>();

            // remind only first and every 5th drink this night to avoid spamming
            if (ShouldNotifyUsers(activity, userStats))
            {
                notifications.AddRange(friendsUserIds.Select(u => new ActivityNotification(u, activity)));
            }

            return notifications;
        }

        private bool ShouldNotifyUsers(Activity activity, UserStatistics userStats)
        {
            bool shouldNotify = (
                (activity.ActivityType == ActivityType.Image || activity.ActivityType == ActivityType.Message ||
                activity.ActivityType == ActivityType.Registration || activity.ActivityType == ActivityType.GameResult) ||
                (userStats == null || userStats.CurrentNightDrinks == 1 || (userStats.CurrentNightDrinks % 5 == 0)));

            return shouldNotify;
        }
    }
}
