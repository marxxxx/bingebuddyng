namespace BingeBuddyNg.Services.Activity
{
    public class ReactionAddedMessage
    {
        public ReactionAddedMessage()
        { }
        public ReactionAddedMessage(string activityId, ReactionType type, string userId, string comment)
        {
            this.ActivityId = activityId;
            this.ReactionType = type;
            this.UserId = userId;
            this.Comment = comment;
        }

        public string ActivityId { get; set; }

        public ReactionType ReactionType { get; set; }
        public string UserId { get; set; }
        public string Comment { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(ActivityId)}={ActivityId}, {nameof(ReactionType)}={ReactionType}, {nameof(UserId)}={UserId}, {nameof(Comment)}={Comment}}}";
        }
    }
}
