using System;

namespace BingeBuddyNg.Core.Activity.Messages
{
    public class DeleteActivityMessage
    {
        public string ActivityId { get; set; }

        public DeleteActivityMessage(string activityId)
        {
            this.ActivityId = activityId ?? throw new ArgumentNullException(nameof(activityId));
        }

        public override string ToString()
        {
            return $"{{{nameof(ActivityId)}={ActivityId}}}";
        }
    }
}
