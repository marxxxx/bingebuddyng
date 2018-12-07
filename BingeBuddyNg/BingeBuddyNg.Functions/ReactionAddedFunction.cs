using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Messages;
using BingeBuddyNg.Services.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BingeBuddyNg.Functions
{
    public static class ReactionAddedFunction
    {

        // we stick with poor man's DI for now
        public static readonly IActivityRepository ActivityRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IActivityRepository>();
        public static readonly IUserRepository UserRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IUserRepository>();
        public static readonly INotificationService NotificationService = ServiceProviderBuilder.Instance.Value.GetRequiredService<INotificationService>();


        [FunctionName("ReactionAddedFunction")]
        public static async Task Run([QueueTrigger(Shared.Constants.QueueNames.ReactionAdded, Connection = "AzureWebJobsStorage")]string reactionQueueItem, ILogger log)
        {
            var reactionAddedMessage = JsonConvert.DeserializeObject<ReactionAddedMessage>(reactionQueueItem);

            var activity = await ActivityRepository.GetActivityAsync(reactionAddedMessage.ActivityId);
            var reactingUser = await UserRepository.FindUserAsync(reactionAddedMessage.UserId);


            // notify involved users
            if (activity.UserId != reactingUser.Id)
            {
                await NotifyOriginUserAsync(reactionAddedMessage.ReactionType, activity.UserId, reactingUser);
            }

            // now other ones (with likes and cheers)
            var involvedUsers = activity.Cheers?.Select(c => new UserInfo(c.UserId, c.UserName))
                .Union(activity.Likes?.Select(l => new UserInfo(l.UserId, l.UserName)))
                .Distinct()
                .Where(u=>u.UserId != activity.UserId && u.UserId != reactingUser.Id)
                .ToList();

            string message = GetReactionMessage(reactionAddedMessage.ReactionType, reactingUser.Name,
                        activity.UserName, false);

            List<PushInfo> involvedUserPushInfos = new List<PushInfo>();
            foreach (var user in involvedUsers)
            {
                var userInfo = await UserRepository.FindUserAsync(user.UserId);
                if(userInfo.PushInfo != null)
                {
                    involvedUserPushInfos.Add(userInfo.PushInfo);
                }
            }

            var notification = new Services.Models.NotificationMessage(Constants.NotificationIconUrl,
                  Constants.NotificationIconUrl, Constants.ApplicationUrl, Constants.ApplicationName, message);
            NotificationService.SendMessage(involvedUserPushInfos, notification);
        }

        private static async Task NotifyOriginUserAsync(
            ReactionType reactionType, string originUserId, User reactingUser)
        {
            var originUser = await UserRepository.FindUserAsync(originUserId);
            
            if (originUser.PushInfo != null)
            {
                string message = GetReactionMessage(reactionType, reactingUser.Name, originUser.Name);
                var notification = new Services.Models.NotificationMessage(Constants.NotificationIconUrl,
                    Constants.NotificationIconUrl, Constants.ApplicationUrl, Constants.ApplicationName, message);
                NotificationService.SendMessage(new[] { originUser.PushInfo }, notification);
            }
        }

        private static string GetReactionMessage(ReactionType reactionType, 
            string reactingUserName, string originUserName, 
            bool isPersonal = true)
        {
            string message = null;

            switch (reactionType)
            {

                case Services.Models.ReactionType.Cheers:
                    message = string.Format("hat {0} zugeprostet.", isPersonal ? "dir" : (originUserName == reactingUserName) ? "sich selbst" : originUserName);
                    break;
                case Services.Models.ReactionType.Like:
                    message = string.Format("gefällt {0} Aktivität.", isPersonal ? "deine" : (originUserName == reactingUserName) ? "seine eigene" : originUserName + "'s");
                    break;
                case Services.Models.ReactionType.Comment:
                    message = string.Format("hat {0} Aktivität kommentiert.", isPersonal ? "deine" : (originUserName == reactingUserName) ? "seine eigene" : originUserName + "'s");
                    break;
            }

            return $"{reactingUserName} {message}";
        }
    }
}
