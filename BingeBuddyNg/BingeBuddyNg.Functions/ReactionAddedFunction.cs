using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Activity.Messages;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Core.User.Persistence;
using BingeBuddyNg.Functions.Services;
using BingeBuddyNg.Functions.Services.Notifications;
using BingeBuddyNg.Shared;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BingeBuddyNg.Functions
{
    public class ReactionAddedFunction
    {
        private readonly IActivityRepository activityRepository;
        private readonly IUserRepository userRepository;
        private readonly PushNotificationService pushNotificationService;
        private readonly ActivityDistributionService activityDistributionService;

        public ReactionAddedFunction(
            IActivityRepository activityRepository, 
            IUserRepository userRepository, 
            PushNotificationService pushNotificationService,
            ActivityDistributionService activityDistributionService)
        {
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.pushNotificationService = pushNotificationService ?? throw new ArgumentNullException(nameof(pushNotificationService));
            this.activityDistributionService = activityDistributionService ?? throw new ArgumentNullException(nameof(activityDistributionService));
        }

        [FunctionName(nameof(ReactionAddedFunction))]
        public async Task Run([QueueTrigger(Shared.Constants.QueueNames.ReactionAdded, Connection = "AzureWebJobsStorage")]string reactionQueueItem, ILogger log)
        {
            var reactionAddedMessage = JsonConvert.DeserializeObject<ReactionAddedMessage>(reactionQueueItem);

            var activity = await activityRepository.GetActivityAsync(reactionAddedMessage.ActivityId);
            var reactingUser = await userRepository.GetUserAsync(reactionAddedMessage.UserId);
            var originUser = await userRepository.GetUserAsync(activity.UserId);

            await this.activityDistributionService.DistributeActivitiesAsync(originUser, activity.ToEntity());

            List<NotificationBase> notifications = new List<NotificationBase>();
            var url = GetActivityUrlWithHighlightedActivityId(activity.Id);

            // notify involved users
            if (activity.UserId != reactingUser.Id)
            {
                var notification = new ReactionNotification(
                        originUser.Id,
                        reactionAddedMessage.ReactionType,
                        reactingUser.Name,
                        originUser.Name,
                        true,
                        url);

                notifications.Add(notification);
            }

            // now other ones (with likes and cheers)
            var involvedUserNotifications = activity.Cheers?.Select(c => new UserInfo(c.UserId, c.UserName))
                .Union(activity.Likes?.Select(l => new UserInfo(l.UserId, l.UserName)))
                .Distinct()
                .Where(u => u.UserId != activity.UserId && u.UserId != reactingUser.Id)
                .Select(u => new ReactionNotification(u.UserId, reactionAddedMessage.ReactionType, reactingUser.Name, activity.UserName, false, url));

            notifications.AddRange(involvedUserNotifications);

            await pushNotificationService.NotifyAsync(notifications);
        }

        private string GetActivityUrlWithHighlightedActivityId(string activityId)
        {
            return $"{Constants.Urls.ApplicationUrl}/activity-feed?activityId={activityId}";
        }
    }
}
