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

namespace BingeBuddyNg.Functions
{
    public class ReactionAddedFunction
    {
        public IActivityRepository ActivityRepository { get; }
        public IUserRepository UserRepository { get; }
        public INotificationService NotificationService { get; }
        public ITranslationService TranslationService { get; }

        public ReactionAddedFunction(IActivityRepository activityRepository, IUserRepository userRepository, 
            INotificationService notificationService, ITranslationService translationService)
        {
            this.ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.NotificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.TranslationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
        }

        [FunctionName("ReactionAddedFunction")]
        public async Task Run([QueueTrigger(Shared.Constants.QueueNames.ReactionAdded, Connection = "AzureWebJobsStorage")]string reactionQueueItem, ILogger log)
        {
            var reactionAddedMessage = JsonConvert.DeserializeObject<ReactionAddedMessage>(reactionQueueItem);

            var activity = await ActivityRepository.GetActivityAsync(reactionAddedMessage.ActivityId);
            var reactingUser = await UserRepository.FindUserAsync(reactionAddedMessage.UserId);


            // notify involved users
            if (activity.UserId != reactingUser.Id)
            {
                await NotifyOriginUserAsync(activity.Id, reactionAddedMessage.ReactionType, activity.UserId, reactingUser);
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
                    var userInfo = await UserRepository.FindUserAsync(user.UserId);
                    if (userInfo.PushInfo != null)
                    {
                        string message = await GetReactionMessageAsync(userInfo.Language, reactionAddedMessage.ReactionType, reactingUser.Name,
                            activity.UserName, false);

                        var notification = new NotificationMessage(Constants.NotificationIconUrl,
                            Constants.NotificationIconUrl, GetActivityUrlWithHighlightedActivityId(activity.Id), Constants.ApplicationName, message);
                        NotificationService.SendWebPushMessage(new[] { userInfo.PushInfo }, notification);
                    }
                }
                catch (Exception ex)
                {
                    log.LogError($"Error notifying user {user}: {ex}");
                }
            }


        }

        private async Task NotifyOriginUserAsync(
            string activityId,
            ReactionType reactionType, string originUserId, User reactingUser)
        {
            var originUser = await UserRepository.FindUserAsync(originUserId);

            if (originUser.PushInfo != null)
            {
                string message = await GetReactionMessageAsync(originUser.Language, reactionType, reactingUser.Name, originUser.Name);
                var notification = new NotificationMessage(Constants.NotificationIconUrl,
                    Constants.NotificationIconUrl, GetActivityUrlWithHighlightedActivityId(activityId), Constants.ApplicationName, message);
                NotificationService.SendWebPushMessage(new[] { originUser.PushInfo }, notification);
            }
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
                    message = await TranslationService.GetTranslationAsync(language, "CheersReactionMessage" + postFix, originUserName);
                    break;
                case ReactionType.Like:
                    message = await TranslationService.GetTranslationAsync(language, "LikeReactionMessage" + postFix, originUserName);
                    break;
                case ReactionType.Comment:
                    message = await TranslationService.GetTranslationAsync(language, "CommentReactionMessage" + postFix, originUserName);
                    break;
            }

            return $"{reactingUserName} {message}";
        }
    }
}
