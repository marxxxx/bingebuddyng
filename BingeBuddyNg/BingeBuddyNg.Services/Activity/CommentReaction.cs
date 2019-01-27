using System;

namespace BingeBuddyNg.Services.Activity
{
    public class CommentReaction : Reaction
    {
        public string Comment { get; set; }

        public CommentReaction()
        {

        }

        public CommentReaction(string userId, string userName, string userProfileImageUrl, string comment) 
            : base(userId, userName, userProfileImageUrl)
        {
            this.Comment = comment;
        }

        public CommentReaction(DateTime timestamp, string userId, string userName, string userProfileImageUrl, string comment)
            :this(userId, userName, userProfileImageUrl, comment)
        {
            this.Timestamp = timestamp;
        }

        public override string ToString()
        {
            return $"{{{base.ToString()}, {nameof(Comment)}={Comment}}}";
        }
    }
}
