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
    public static class ReactionAddedFunction
    {

        // we stick with poor man's DI for now
        public static readonly IActivityRepository ActivityRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IActivityRepository>();
        public static readonly IUserRepository UserRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IUserRepository>();
        public static readonly INotificationService NotificationService = ServiceProviderBuilder.Instance.Value.GetRequiredService<INotificationService>();
        public static readonly ITranslationService TranslationService = ServiceProviderBuilder.Instance.Value.GetRequiredService<ITranslationService>();


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
                            Constants.NotificationIconUrl, Constants.ApplicationUrl, Constants.ApplicationName, message);
                        NotificationService.SendMessage(new[] { userInfo.PushInfo }, notification);
                    }
                }
                catch (Exception ex)
                {
                    log.LogError($"Error notifying user {user}: {ex}");
                }
            }


        }

        private static async Task NotifyOriginUserAsync(
            ReactionType reactionType, string originUserId, User reactingUser)
        {
            var originUser = await UserRepository.FindUserAsync(originUserId);

            if (originUser.PushInfo != null)
            {
                string message = await GetReactionMessageAsync(originUser.Language, reactionType, reactingUser.Name, originUser.Name);
                var notification = new NotificationMessage(Constants.NotificationIconUrl,
                    Constants.NotificationIconUrl, Constants.ApplicationUrl, Constants.ApplicationName, message);
                NotificationService.SendMessage(new[] { originUser.PushInfo }, notification);
            }
        }

        private static async Task<string> GetReactionMessageAsync(
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
