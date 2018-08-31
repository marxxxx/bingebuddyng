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
                // get all users
                var users = await userRepository.GetAllUsersAsync();

                // Immediately update Stats for current user
                var currentUser = users.First(u => u.Id == activity.UserId);
                await DrinkCalculatorFunction.UpdateStatsForUserasync(currentUser, calculationService, userStatsRepository, log);

                // send out push

                var otherUsersWithPushInfo = users.Where(u => u.PushInfo != null && u.Id != activity.UserId)
                    .Select(u => u.PushInfo).ToList();

                string locationSnippet = null;
                if (!string.IsNullOrEmpty(activity.LocationAddress))
                {
                    locationSnippet = $" in {activity.LocationAddress} ";
                }

                // TODO: Localize
                var notificationMessage = new NotificationMessage(Constants.NotificationIconUrl, "BingeBuddy",
                    $"{activityAddedMessage.AddedActivity.UserName} hat {activity.DrinkName}{locationSnippet} geschnappt!");

                notificationService.SendMessage(otherUsersWithPushInfo, notificationMessage);
            }
            catch (Exception ex)
            {
                log.LogError($"Failed to send push notification: [{ex}]");
            }

        }
    }
}
