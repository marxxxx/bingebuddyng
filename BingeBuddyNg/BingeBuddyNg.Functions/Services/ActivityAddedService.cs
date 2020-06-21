using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.Statistics;
using BingeBuddyNg.Core.Statistics.Commands;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Core.User.Domain;
using BingeBuddyNg.Functions.Services.Notifications;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Shared;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace BingeBuddyNg.Functions.Services
{
    public class ActivityAddedService
    {
        private readonly IAddressDecodingService utilityService;
        private readonly IActivityRepository activityRepository;
        private readonly IUserRepository userRepository;
        private readonly INotificationService notificationService;
        private readonly UpdateRankingCommand updateRankingCommand;
        private readonly UpdateStatisticsCommand updateStatisticsCommand;
        private readonly ActivityDistributionService activityDistributionService;
        private readonly DrinkEventHandlingService drinkEventHandlingService;
        private readonly PushNotificationService pushNotificationService;
        private readonly ILogger<ActivityAddedService> logger;

        public ActivityAddedService(
            IAddressDecodingService utilityService,
            IActivityRepository activityRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            UpdateRankingCommand updateRankingCommand,
            UpdateStatisticsCommand updateStatisticsCommand,
            ActivityDistributionService activityDistributionService,
            DrinkEventHandlingService drinkEventHandlingService,
            PushNotificationService pushNotificationService,
            ILogger<ActivityAddedService> logger)
        {
            this.utilityService = utilityService;
            this.activityRepository = activityRepository;
            this.userRepository = userRepository;
            this.notificationService = notificationService;
            this.updateRankingCommand = updateRankingCommand;
            this.updateStatisticsCommand = updateStatisticsCommand;
            this.activityDistributionService = activityDistributionService;
            this.drinkEventHandlingService = drinkEventHandlingService;
            this.pushNotificationService = pushNotificationService;
            this.logger = logger;
        }

        public async Task RunAsync(ActivityAddedMessage message, IDurableOrchestrationClient durableClient)
        {
            var activity = await activityRepository.GetActivityAsync(message.ActivityId);

            logger.LogInformation($"Handling added activity [{activity}] ...");

            var currentUser = await userRepository.GetUserAsync(activity.UserId);

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
                        userStats = await updateStatisticsCommand.ExecuteAsync(currentUser.Id, currentUser.Gender, currentUser.Weight);
                        await updateRankingCommand.ExecuteAsync(currentUser.Id);

                        activity.UpdateStats(userStats.CurrentNightDrinks, userStats.CurrentAlcoholization);

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

                var entity = activity.ToEntity();

                if (shouldUpdate)
                {
                    await activityRepository.UpdateActivityAsync(entity);
                }

                await this.activityDistributionService.DistributeActivitiesAsync(currentUser, entity);

                await SendActivityUpdateAsync(currentUser, activity, userStats);

                // check for drink events
                try
                {
                    if(activity.ActivityType == ActivityType.Drink)
                    {
                        await this.drinkEventHandlingService.HandleDrinkEventsAsync(activity, currentUser);
                    }                    
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
                currentUser.UpdateMonitoringInstance(monitoringInstanceId);
                await userRepository.UpdateUserAsync(currentUser.ToEntity());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error managing monitoring function");
            }
        }

        private async Task HandleLocationUpdateAsync(Activity activity)
        {
            var address = await utilityService.GetAddressFromLongLatAsync(activity.Location);
            activity.UpdateLocation(address.AddressText, address.CountryShortName, address.CountryLongName);
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
