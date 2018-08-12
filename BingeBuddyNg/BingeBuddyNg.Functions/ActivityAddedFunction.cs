using System;
using System.Threading.Tasks;
using BingeBuddyNg.Functions.DependencyInjection;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Messages;
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


            
        }
    }
}
