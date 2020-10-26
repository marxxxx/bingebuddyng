using System;

namespace BingeBuddyNg.Core.Activity.Messages
{
    public class ActivityAddedMessage
    {
        public string ActivityId { get; set; }

        public ActivityAddedMessage()
        { }

        public ActivityAddedMessage(string activityId)
        {
            this.ActivityId = activityId ?? throw new ArgumentNullException(nameof(activityId));
        }
    }
}