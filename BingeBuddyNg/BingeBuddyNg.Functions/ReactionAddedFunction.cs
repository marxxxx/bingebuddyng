using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using System.Collections.Generic;

namespace BingeBuddyNg.Functions
{
    public class ReactionAddedFunction
    {
        private readonly IActivityRepository activityRepository;
        private readonly IUserRepository userRepository;
        private readonly INotificationService notificationService;
        private readonly ITranslationService translationService;

        public ReactionAddedFunction(IActivityRepository activityRepository, IUserRepository userRepository, 
            INotificationService notificationService, ITranslationService translationService)
        {
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.translationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
        }

        [FunctionName("ReactionAddedFunction")]
        public async Task Run([QueueTrigger(Shared.Constants.QueueNames.ReactionAdded, Connection = "AzureWebJobsStorage")]string reactionQueueItem, ILogger log)
        {
            var reactionAddedMessage = JsonConvert.DeserializeObject<ReactionAddedMessage>(reactionQueueItem);

            var activity = await activityRepository.GetActivityAsync(reactionAddedMessage.ActivityId);
            var reactingUser = await userRepository.FindUserAsync(reactionAddedMessage.UserId);
            var originUser = await userRepository.FindUserAsync(activity.UserId);

            await DistributeActivitiesAsync(originUser, activity);

            // notify involved users
            if (activity.UserId != reactingUser.Id)
            {
                await NotifyOriginUserAsync(activity.Id, reactionAddedMessage.ReactionType, originUser, reactingUser);
            }

            // now other ones (with likes and cheers)
            var involvedUsers = activity.Cheers?.Select(c => new UserInfo(c.UserId, c.UserName))
                .Union(activity.Likes?.Select(l => new UserInfo(l.UserId, l.UserName)))
                .Distinct()
                .Where(u => u.UserId != activity.UserId && u.UserId != reactingUser.Id)
                .ToList();

            foreach (var user in involvedUsers)
            {
                try
                {
                    var userInfo = await userRepository.FindUserAsync(user.UserId);
                    if (userInfo.PushInfo != null)
                    {
                        string message = await GetReactionMessageAsync(userInfo.Language, reactionAddedMessage.ReactionType, reactingUser.Name,
                            activity.UserName, false);

                        var notification = new NotificationMessage(Constants.NotificationIconUrl,
                            Constants.NotificationIconUrl, GetActivityUrlWithHighlightedActivityId(activity.Id), Constants.ApplicationName, message);
                        notificationService.SendWebPushMessage(new[] { userInfo.PushInfo }, notification);
                    }
                }
                catch (Exception ex)
                {
                    log.LogError($"Error notifying user {user}: {ex}");
                }
            }
        }

        private async Task DistributeActivitiesAsync(User currentUser, Activity activity)
        {
            IEnumerable<string> userIds;

            if (activity.ActivityType == ActivityType.Registration)
            {
                userIds = await this.userRepository.GetAllUserIdsAsync();
            }
            else
            {
                // get friends of this user who didn't mute themselves from him
                userIds = currentUser.GetVisibleFriendUserIds(true);
            }

            await this.activityRepository.DistributeActivityAsync(userIds, activity);
        }

        private async Task NotifyOriginUserAsync(
            string activityId,
            ReactionType reactionType, User originUser, User reactingUser)
        {
            if (originUser.PushInfo == null)
            {
                return;
            }

            string message = await GetReactionMessageAsync(originUser.Language, reactionType, reactingUser.Name, originUser.Name);
            var notification = new NotificationMessage(Constants.NotificationIconUrl,
                Constants.NotificationIconUrl, GetActivityUrlWithHighlightedActivityId(activityId), Constants.ApplicationName, message);
            notificationService.SendWebPushMessage(new[] { originUser.PushInfo }, notification);
        }

        private string GetActivityUrlWithHighlightedActivityId(string activityId)
        {
            return $"{Constants.ApplicationUrl}/activity-feed?activityId={activityId}";
        }

        private async Task<string> GetReactionMessageAsync(
            string language,
            ReactionType reactionType,
            string reactingUserName, string originUserName,
            bool isPersonal = true)
        {
            string message = null;
            string postFix = null;
            if (isPersonal)
            {
                postFix = "Your";
            }
            else if (originUserName == reactingUserName)
            {
                postFix = "Self";
            }
            else
            {
                postFix = "Other";
            }


            switch (reactionType)
            {
                case ReactionType.Cheers:
                    message = await translationService.GetTranslationAsync(language, "CheersReactionMessage" + postFix, originUserName);
                    break;
                case ReactionType.Like:
                    message = await translationService.GetTranslationAsync(language, "LikeReactionMessage" + postFix, originUserName);
                    break;
                case ReactionType.Comment:
                    message = await translationService.GetTranslationAsync(language, "CommentReactionMessage" + postFix, originUserName);
                    break;
            }

            return $"{reactingUserName} {message}";
        }
    }
}
