using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Messages
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
