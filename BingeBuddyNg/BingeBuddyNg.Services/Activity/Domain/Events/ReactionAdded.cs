using System;

namespace BingeBuddyNg.Services.Activity.Domain.Events
{
    public class ReactionAdded : IDomainEvent
    {
        public ReactionAdded(string activityId, ReactionType reactionType, string userId)
        {
            this.ActivityId = activityId ?? throw new ArgumentNullException(nameof(activityId));
            this.ReactionType = reactionType;
            this.UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        public ReactionAdded(string activityId, ReactionType reactionType, string userId, string comment)
        {
            this.ActivityId = activityId ?? throw new ArgumentNullException(nameof(activityId));
            this.ReactionType = reactionType;
            this.UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            this.Comment = comment ?? throw new ArgumentNullException(nameof(comment));
        }

        public string ActivityId { get; set; }

        public ReactionType ReactionType { get; set; }

        public string UserId { get; set; }

        public string Comment { get; set; }
    }
}
