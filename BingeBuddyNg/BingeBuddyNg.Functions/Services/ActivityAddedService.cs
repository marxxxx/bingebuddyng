﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.Activity.DTO;
using BingeBuddyNg.Core.Activity.Messages;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.Statistics;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Core.User.Domain;
using BingeBuddyNg.Functions.Services.Notifications;
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
        private readonly IMonitoringRepository monitoringRepository;
        private readonly UserStatisticUpdateService statisticUpdateService;
        private readonly ActivityDistributionService activityDistributionService;
        private readonly DrinkEventHandlingService drinkEventHandlingService;
        private readonly PushNotificationService pushNotificationService;
        private readonly ILogger<ActivityAddedService> logger;

        public ActivityAddedService(
            IAddressDecodingService utilityService,
            IActivityRepository activityRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IMonitoringRepository monitoringRepository,
            UserStatisticUpdateService rankingService,
            ActivityDistributionService activityDistributionService,
            DrinkEventHandlingService drinkEventHandlingService,
            PushNotificationService pushNotificationService,
            ILogger<ActivityAddedService> logger)
        {
            this.utilityService = utilityService;
            this.activityRepository = activityRepository;
            this.userRepository = userRepository;
            this.notificationService = notificationService;
            this.monitoringRepository = monitoringRepository;
            this.statisticUpdateService = rankingService;
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
                // Notifying user makes only sense when he provided push infos
                if (activity.ActivityType == ActivityType.Drink && currentUser.PushInfo != null)
                {
                    await HandleMonitoringAsync(durableClient, currentUser.Id);
                }

                bool shouldUpdate = false;
                UserStatistics userStats = null;
                try
                {
                    // Immediately update Stats for current user
                    userStats = await statisticUpdateService.UpdateStatisticsAsync(currentUser.Id, currentUser.Gender, currentUser.Weight);

                    if (activity.ActivityType == ActivityType.Drink)
                    {
                        await statisticUpdateService.UpdateRankingAsync(currentUser.Id);

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
                    if (activity.ActivityType == ActivityType.Drink)
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

        private async Task HandleMonitoringAsync(IDurableOrchestrationClient durableClient, string currentUserId)
        {
            try
            {
                var entity = await this.monitoringRepository.FindAsync(currentUserId);
                if (entity != null && entity.MonitoringInstanceId != null)
                {
                    await durableClient.TerminateAsync(entity.MonitoringInstanceId, "Drank early enough.");
                }

                // Start timer to remind user about entering his next drink.
                var monitoringInstanceId = await durableClient.StartNewAsync(nameof(DrinkReminderFunction), currentUserId);
                await this.monitoringRepository.SaveAsync(currentUserId, monitoringInstanceId);
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