using System;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.DTO;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Messages;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BingeBuddyNg.Functions
{
    public static class ProfileUpdateFunction
    {
        [FunctionName("ProfileUpdateFunction")]
        public static async Task Run([QueueTrigger("profile-update", Connection = "AzureWebJobsStorage")]string queueItem, ILogger log)
        {
            var message = JsonConvert.DeserializeObject<ProfileUpdateMessage>(queueItem);
            log.LogInformation($"Updating profile information for user {message.UserId} based on message {queueItem} ...");

            // we stick with poor man's DI for now
            IActivityRepository activityRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IActivityRepository>();
            IUserRepository userRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IUserRepository>();

            var args = new GetActivityFilterArgs(new string[] { message.UserId }, null) { PageSize = 100 };
            var userActivities = await activityRepository.GetActivityFeedAsync(args);
            do
            {
                foreach(var ua in userActivities.ResultPage)
                {
                    if (ua.UserId == message.UserId && ua.UserProfileImageUrl != message.UserProfileImageUrl)
                    {
                        ua.UserProfileImageUrl = message.UserProfileImageUrl;
                        await activityRepository.UpdateActivityAsync(ua);
                    }
                }

                args.SetContinuationToken(userActivities.ContinuationToken);
                userActivities = await activityRepository.GetActivityFeedAsync(args);
            } while (userActivities.ContinuationToken != null);

            // update friends
            var user = await userRepository.FindUserAsync(message.UserId);
            foreach(var friend in user.Friends)
            {
                var friendUser = await userRepository.FindUserAsync(friend.UserId);
                if(friendUser != null)
                {
                    var changedFriend = friendUser.Friends.FirstOrDefault(f => f.UserId == message.UserId);
                    if(changedFriend != null && changedFriend.UserProfileImageUrl != message.UserProfileImageUrl)
                    {
                        changedFriend.UserProfileImageUrl = message.UserProfileImageUrl;
                        await userRepository.UpdateUserAsync(friendUser);
                    }
                }
            }

            log.LogInformation($"Successfully updated profile information for user {message.UserId}");
        }
    }
}
