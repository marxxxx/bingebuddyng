using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Activity.Messages
{
    public class DeleteActivityMessage
    {
        public DeleteActivityMessage(string activityId)
        {
            this.ActivityId = activityId ?? throw new ArgumentNullException(nameof(activityId));
        }

        public string ActivityId { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(ActivityId)}={ActivityId}}}";
        }
    }
}
