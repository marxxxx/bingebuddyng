using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Messages;
using BingeBuddyNg.Services.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BingeBuddyNg.Functions
{
    public static class ActivityAddedFunction
    {
        [FunctionName("ActivityAddedFunction")]
        public static async Task Run([QueueTrigger("activity-added", Connection = "AzureWebJobsStorage")]string message,
            ILogger log)
        {
            // we stick with poor man's DI for now
            IUtilityService utilityService = ServiceProviderBuilder.Instance.Value.GetRequiredService<IUtilityService>();
            IActivityRepository activityRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IActivityRepository>();
            IUserRepository userRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IUserRepository>();
            ICalculationService calculationService = ServiceProviderBuilder.Instance.Value.GetRequiredService<ICalculationService>();
            IUserStatsRepository userStatsRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IUserStatsRepository>();
            INotificationService notificationService = ServiceProviderBuilder.Instance.Value.GetRequiredService<INotificationService>();

            var activityAddedMessage = JsonConvert.DeserializeObject<ActivityAddedMessage>(message);
            var activity = activityAddedMessage.AddedActivity;

            if (activity.Location != null && activity.Location.IsValid())
            {
                var address = await utilityService.GetAddressFromLongLatAsync(activity.Location);
                activity.LocationAddress = address.AddressText;
                activity.CountryLongName = address.CountryLongName;
                activity.CountryShortName = address.CountryShortName;

                await activityRepository.UpdateActivityAsync(activity);
            }

            try
            {
                var currentUser = await userRepository.FindUserAsync(activity.UserId);

                // Immediately update Stats for current user
                await DrinkCalculatorFunction.UpdateStatsForUserasync(currentUser, calculationService, userStatsRepository, log);

                // get friends of this user
                foreach (var friend in currentUser?.Friends)
                {
                    try
                    {
                        var friendUser = await userRepository.FindUserAsync(friend.UserId);
                        if (friendUser != null && friendUser.PushInfo != null)
                        {
                            // TODO: Localize
                            var notificationMessage = GetNotificationMessage(activity);

                            notificationService.SendMessage(new[] { friendUser.PushInfo }, notificationMessage);
                        }
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Failed to send push notification: [{ex}]");
            }

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
                    activityString = $"ein Foto hochgeladen!";
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
