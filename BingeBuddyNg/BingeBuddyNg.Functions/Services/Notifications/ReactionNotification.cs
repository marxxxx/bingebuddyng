using System;
using BingeBuddyNg.Core.Activity.Domain;

namespace BingeBuddyNg.Functions.Services.Notifications
{
    public class ReactionNotification : NotificationBase
    {
        public ReactionType ReactionType { get; }

        public string ReactingUserName { get; }

        public string OriginUserName { get; }

        public bool IsPersonal { get; }

        public ReactionNotification(
            string userId,
            ReactionType reactionType,
            string reactingUserName,
            string originUserName,
            bool isPersonal,
            string url)
            : base(userId, url)
        {
            this.ReactionType = reactionType;
            this.ReactingUserName = reactingUserName ?? throw new ArgumentNullException(nameof(reactingUserName));
            this.OriginUserName = originUserName ?? throw new ArgumentNullException(nameof(originUserName));
            this.IsPersonal = isPersonal;
        }
    }
}