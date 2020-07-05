using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Functions.Services.Notifications;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Shared;
using Microsoft.Extensions.Logging;

namespace BingeBuddyNg.Functions.Services
{
    public class PushNotificationService
    {
        private readonly IUserRepository userRepository;
        private readonly INotificationService notificationService;
        private readonly ITranslationService translationService;
        private readonly ILogger<PushNotificationService> logger;


        public PushNotificationService(
            IUserRepository userRepository,
            INotificationService notificationService, 
            ITranslationService translationService, 
            ILogger<PushNotificationService> logger)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.translationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task NotifyAsync(IEnumerable<NotificationBase> notifications)
        {
            var notificationPerUser = notifications.GroupBy(n => n.UserId);

            foreach (var userNotifications in notificationPerUser)
            {
                var user = await this.userRepository.GetUserAsync(userNotifications.Key);
                if (user.PushInfo == null ||
                    user.LastOnline <= DateTime.UtcNow.Subtract(TimeSpan.FromDays(30)))
                {
                    continue;
                }

                foreach (var n in userNotifications)
                {
                    NotificationMessage message = null;
                    switch (n)
                    {
                        case ActivityNotification activity:
                            {
                                message = await BuildNotificationMessageAsync(user.Language, activity);
                                break;
                            }
                        case ReactionNotification reaction:
                            {
                                message = await BuildNotificationMessageAsync(user.Language, reaction);
                                break;
                            }
                        case DrinkEventCongratulationNotification congrats:
                            {
                                message = await BuildNotificationMessageAsync(user.Language, congrats);
                                break;
                            }
                        case DrinkEventNotification drinkEvent:
                            {
                                message = await BuildNotificationMessageAsync(user.Language, drinkEvent);
                                break;
                            }

                        default:
                            {
                                this.logger.LogError($"Unsupported notification message [{message}].");
                                continue;
                            }
                    }

                    var webPushMessage = new WebPushNotificationMessage(
                        Constants.Urls.ApplicationIconUrl,
                        Constants.Urls.ApplicationIconUrl,
                        Constants.Urls.ApplicationUrl,
                        message.Title,
                        message.Body);

                    notificationService.SendWebPushMessage(new[] { user.PushInfo }, webPushMessage);
                }
            }
        }

        private async Task<NotificationMessage> BuildNotificationMessageAsync(string language, ActivityNotification notification)
        {
            var activity = notification.Activity;

            string locationSnippet = null;
            if (!string.IsNullOrEmpty(activity.LocationAddress))
            {
                locationSnippet = await translationService.GetTranslationAsync(language, "DrinkLocationSnippet", activity.LocationAddress);
            }

            string activityString = null;
            switch (activity.ActivityType)
            {
                case ActivityType.Drink:
                    activityString = await translationService.GetTranslationAsync(language, "DrinkActivityMessage", activity.Drink.DrinkName, locationSnippet);
                    break;
                case ActivityType.Image:
                    activityString = await translationService.GetTranslationAsync(language, "ImageActivityMessage");
                    break;
                case ActivityType.Message:
                    activityString = await translationService.GetTranslationAsync(language, "MessageActivityMessage", activity.Message.Message);
                    break;
                case ActivityType.VenueEntered:
                    activityString = await translationService.GetTranslationAsync(language, "VenueEnterActivityMessage", activity.Venue.Name);
                    break;
                case ActivityType.VenueLeft:
                    activityString = await translationService.GetTranslationAsync(language, "VenueLeaveActivityMessage", activity.Venue.Name);
                    break;
            }

            return new NotificationMessage(Constants.ApplicationName, $"{activity.UserName} {activityString}");
        }


        private async Task<NotificationMessage> BuildNotificationMessageAsync(string language, ReactionNotification notification)
        {
            string message = null;
            string postFix;
            if (notification.IsPersonal)
            {
                postFix = "Your";
            }
            else if (notification.OriginUserName == notification.ReactingUserName)
            {
                postFix = "Self";
            }
            else
            {
                postFix = "Other";
            }

            switch (notification.ReactionType)
            {
                case ReactionType.Cheers:
                    message = await translationService.GetTranslationAsync(language, "CheersReactionMessage" + postFix, notification.OriginUserName);
                    break;
                case ReactionType.Like:
                    message = await translationService.GetTranslationAsync(language, "LikeReactionMessage" + postFix, notification.OriginUserName);
                    break;
                case ReactionType.Comment:
                    message = await translationService.GetTranslationAsync(language, "CommentReactionMessage" + postFix, notification.OriginUserName);
                    break;
            }

            return new NotificationMessage(Constants.ApplicationName, $"{notification.ReactingUserName} {message}");
        }

        private async Task <NotificationMessage> BuildNotificationMessageAsync(string language, DrinkEventCongratulationNotification congrats)
        {
            string message = await translationService.GetTranslationAsync(language, "DrinkEventWinMessage", Constants.Scores.StandardDrinkAction);

            return new NotificationMessage("Gratuliere!", message);
        }

        private async Task<NotificationMessage> BuildNotificationMessageAsync(string language, DrinkReminderNotification reminder)
        {
            var title = await translationService.GetTranslationAsync(language, "DrinkReminder");
            var body = await translationService.GetTranslationAsync(language, "DrinkReminderMessage");

            return new NotificationMessage(title, body);
        }

        private async Task<NotificationMessage> BuildNotificationMessageAsync(string language, DrinkEventNotification drinkEvent)
        {
            var title = await translationService.GetTranslationAsync(language, "DrinkEvent");
            var body = await translationService.GetTranslationAsync(language, "DrinkEventNotificationMessage", Shared.Constants.Scores.StandardDrinkAction);

            return new NotificationMessage(title, body);
        }
    }
}
