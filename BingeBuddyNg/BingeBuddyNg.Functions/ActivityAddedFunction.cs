using System;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Functions.DependencyInjection;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Messages;
using BingeBuddyNg.Services.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BingeBuddyNg.Functions
{
    public static class ActivityAddedFunction
    {
        [FunctionName("ActivityAddedFunction")]
        public static async Task Run([QueueTrigger("activity-added", Connection = "AzureWebJobsStorage")]string message, 
            [Inject]IUtilityService utilityService,
            [Inject]IActivityRepository activityRepository,
            [Inject]IUserRepository userRepository,
            [Inject]INotificationService notificationService,
            [Inject]ICalculationService calculationService,
            [Inject]IUserStatsRepository userStatsRepository,
            ILogger log)
        {
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
                if(!string.IsNullOrEmpty(activity.LocationAddress))
                {
                    locationSnippet = $" in {activity.LocationAddress} ";
                }

                // TODO: Localize, configure icon
                string iconUrl = "https://bingebuddystorage.z6.web.core.windows.net/favicon.ico";
                var notificationMessage = new NotificationMessage(iconUrl, "BingeBuddy", 
                    $"{activityAddedMessage.AddedActivity.UserName} hat {activity.DrinkName}{locationSnippet} geschnappt!");
#if !DEBUG
                notificationService.SendMessage(otherUsersWithPushInfo, notificationMessage);
#endif
            }
            catch(Exception ex)
            {
                log.LogError($"Failed to send push notification: [{ex}]");
            }

        }
    }
}
