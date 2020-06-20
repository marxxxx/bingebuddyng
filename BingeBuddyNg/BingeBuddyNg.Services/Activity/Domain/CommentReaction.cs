using System;

namespace BingeBuddyNg.Core.Activity.Domain
{
    public class CommentReaction : Reaction
    {
        public string Comment { get; set; }

        public CommentReaction()
        {

        }

        public CommentReaction(string userId, string userName, string comment) 
            : base(userId, userName)
        {
            this.Comment = comment;
        }

        public CommentReaction(DateTime timestamp, string userId, string userName, string comment)
            :this(userId, userName, comment)
        {
            this.Timestamp = timestamp;
        }

        public override string ToString()
        {
            return $"{{{base.ToString()}, {nameof(Comment)}={Comment}}}";
        }
    }
}
