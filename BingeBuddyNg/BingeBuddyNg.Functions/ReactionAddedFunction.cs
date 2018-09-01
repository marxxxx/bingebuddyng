using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Messages;
using BingeBuddyNg.Services.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BingeBuddyNg.Functions
{
    public static class ReactionAddedFunction
    {
        [FunctionName("ReactionAddedFunction")]
        public static async Task Run([QueueTrigger("reaction-added", Connection = "AzureWebJobsStorage")]string reactionQueueItem, ILogger log)
        {
            var reactionAddedMessage = JsonConvert.DeserializeObject<ReactionAddedMessage>(reactionQueueItem);

            // we stick with poor man's DI for now
            IActivityRepository activityRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IActivityRepository>();
            IUserRepository userRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IUserRepository>();
            INotificationService notificationService = ServiceProviderBuilder.Instance.Value.GetRequiredService<INotificationService>();

            var activity = await activityRepository.GetActivityAsync(reactionAddedMessage.ActivityId);
            var reactingUser = await userRepository.GetUserAsync(reactionAddedMessage.UserId);

            string message = null;
            

            switch (reactionAddedMessage.ReactionType)
            {

                case Services.Models.ReactionType.Cheers:
                    activity.AddCheers(new Reaction(reactionAddedMessage.UserId, reactingUser.Name, reactingUser.ProfileImageUrl));
                    message = "hat dir zugeprostet.";
                    break;
                case Services.Models.ReactionType.Like:
                    activity.AddLike(new Reaction(reactionAddedMessage.UserId, reactingUser.Name, reactingUser.ProfileImageUrl));
                    message = "gefällt deine Aktivität.";
                    break;
                case Services.Models.ReactionType.Comment:
                    activity.AddComment(new CommentReaction(reactionAddedMessage.UserId, reactingUser.Name, reactingUser.ProfileImageUrl,  reactionAddedMessage.Comment));
                    message = "hat deine Aktivität kommentiert.";
                    break;
            }

            await activityRepository.UpdateActivityAsync(activity);

            var originUser = await userRepository.GetUserAsync(activity.UserId);

            if (originUser.PushInfo != null)
            {
                message = $"{reactingUser.Name} {message}";
                var notification = new Services.Models.NotificationMessage(Constants.NotificationIconUrl, 
                    reactingUser.ProfileImageUrl, Constants.ApplicationUrl, Constants.ApplicationName, message);
                notificationService.SendMessage(new[] { originUser.PushInfo }, notification);
            }

        }
    }
}
