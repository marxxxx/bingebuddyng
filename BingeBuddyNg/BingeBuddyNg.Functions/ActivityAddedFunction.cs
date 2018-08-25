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
                // send out push
                var users = await userRepository.GetAllUsersAsync();

                var otherUsersWithPushInfo = users.Where(u => u.PushInfo != null && u.Id != activity.UserId)
                    .Select(u => u.PushInfo).ToList();

                // TODO: Localize, configure icon
                string iconUrl = "https://bingebuddystorage.z6.web.core.windows.net/favicon.ico";
                var notificationMessage = new NotificationMessage(iconUrl, "BingeBuddy", 
                    $"{activityAddedMessage.AddedActivity.UserName} hat ein {activity.DrinkName} geschnappt!");

                notificationService.SendMessage(otherUsersWithPushInfo, notificationMessage);
            }
            catch(Exception ex)
            {
                log.LogError($"Failed to send push notification: [{ex}]");
            }

        }
    }
}
