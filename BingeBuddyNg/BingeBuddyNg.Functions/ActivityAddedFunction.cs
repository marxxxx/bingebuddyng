using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Messages;
using BingeBuddyNg.Services.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BingeBuddyNg.Functions
{
    public static class ActivityAddedFunction
    {
        // we stick with poor man's DI for now
        public static readonly IUtilityService UtilityService = ServiceProviderBuilder.Instance.Value.GetRequiredService<IUtilityService>();
        public static readonly IActivityRepository ActivityRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IActivityRepository>();
        public static readonly IUserRepository UserRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IUserRepository>();
        public static readonly ICalculationService CalculationService = ServiceProviderBuilder.Instance.Value.GetRequiredService<ICalculationService>();
        public static readonly IUserStatsRepository UserStatsRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IUserStatsRepository>();
        public static readonly INotificationService NotificationService = ServiceProviderBuilder.Instance.Value.GetRequiredService<INotificationService>();
        public static readonly IDrinkEventRepository DrinkEventRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IDrinkEventRepository>();


        [FunctionName("ActivityAddedFunction")]
        public static async Task Run(
            [QueueTrigger(Shared.Constants.QueueNames.ActivityAdded, Connection = "AzureWebJobsStorage")]string message,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {

            var activityAddedMessage = JsonConvert.DeserializeObject<ActivityAddedMessage>(message);
            var activity = activityAddedMessage.AddedActivity;

            if (activity.Location != null && activity.Location.IsValid())
            {
                var address = await UtilityService.GetAddressFromLongLatAsync(activity.Location);
                activity.LocationAddress = address.AddressText;
                activity.CountryLongName = address.CountryLongName;
                activity.CountryShortName = address.CountryShortName;

                await ActivityRepository.UpdateActivityAsync(activity);
            }


            try
            {
                User currentUser = null;
                try
                {
                    currentUser = await UserRepository.FindUserAsync(activity.UserId);

                    if (currentUser.MonitoringInstanceId != null)
                    {
                        await starter.TerminateAsync(currentUser.MonitoringInstanceId, "Drank early enough.");
                    }

                    // Start timer to remind user about entering his next drink.
                    var monitoringInstanceId = await starter.StartNewAsync(DrinkReminderFunction.FunctionNameValue, currentUser);
                    await UserRepository.UpdateMonitoringInstanceAsync(currentUser.Id, monitoringInstanceId);
                }
                catch (Exception ex)
                {
                    log.LogError($"Error managing monitoring function", ex);
                }

                UserStatistics userStats = null;
                try
                {
                    // Immediately update Stats for current user
                    userStats = await DrinkCalculatorFunction.UpdateStatsForUserAsync(currentUser, log);
                    await RankingCalculatorFunction.UpdateRankingForUserAsync(currentUser.Id, log);
                }
                catch (Exception ex)
                {
                    log.LogError($"Failed to update stats for user [{currentUser}]: [{ex}]");
                }

                // remind only first and every 5th drink this night to avoid spamming
                if (ShouldNotifyUsers(activity, userStats))
                {
                    // get friends of this user who didn't mute themselves from him
                    var friendUserIds = currentUser.GetVisibleFriendUserIds(false);

                    foreach (var friendUserId in friendUserIds)
                    {
                        try
                        {
                            var friendUser = await UserRepository.FindUserAsync(friendUserId);
                            if (friendUser != null && friendUser.PushInfo != null)
                            {
                                log.LogInformation($"Sending push to [{friendUser}] ...");

                                // TODO: Localize
                                var notificationMessage = GetNotificationMessage(activity);

                                NotificationService.SendMessage(new[] { friendUser.PushInfo }, notificationMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.LogError($"Failed to send push notification to user [{friendUserId}]: [{ex}]");
                        }
                    }
                }

                // check for drink events
                try
                {
                    if (activity.ActivityType == ActivityType.Drink || activity.DrinkType != DrinkType.Anti)
                    {
                        var drinkEvent = await DrinkEventRepository.FindCurrentDrinkEventAsync();
                        if (drinkEvent != null)
                        {
                            if (drinkEvent.AddScoringUserId(currentUser.Id))
                            {
                                await UserStatsRepository.IncreaseScoreAsync(currentUser.Id, Shared.Constants.Scores.StandardDrinkAction);

                                if (currentUser.PushInfo != null)
                                {
                                    NotificationService.SendMessage(new[] { currentUser.PushInfo }, new NotificationMessage("Gratuliere!", $"Du hast bei der Drink Aktion gewonnen und dir dabei {Shared.Constants.Scores.StandardDrinkAction} verdient!"));
                                }
                            }
                        }
                    }
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

        private static bool ShouldNotifyUsers(Activity activity, UserStatistics userStats)
        {
            bool shouldNotify = ((activity.ActivityType == ActivityType.Image || activity.ActivityType == ActivityType.Message) ||
                (userStats == null || userStats.CurrentNightDrinks == 1 || (userStats.CurrentNightDrinks % 5 == 0)));
            return shouldNotify;
        }

        private static NotificationMessage GetNotificationMessage(Activity activity)
        {
            string locationSnippet = null;
            if (!string.IsNullOrEmpty(activity.LocationAddress))
            {
                locationSnippet = $" in {activity.LocationAddress} ";
            }

            string activityString = null;
            switch (activity.ActivityType)
            {
                case ActivityType.Drink:
                    activityString = $"{activity.DrinkName}{locationSnippet} geschnappt!";
                    break;
                case ActivityType.Image:
                    activityString = $"ein Foto oder Video hochgeladen!";
                    break;
                case ActivityType.Message:
                    activityString = $"{activity.Message} gesagt!";
                    break;
            }

            var notificationMessage = new NotificationMessage(Constants.NotificationIconUrl,
                Constants.NotificationIconUrl, Constants.ApplicationUrl, Constants.ApplicationName,
                    $"{activity.UserName} hat {activityString}");
            return notificationMessage;

        }
    }
}
