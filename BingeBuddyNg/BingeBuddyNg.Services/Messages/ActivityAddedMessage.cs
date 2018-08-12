using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Messages
{
    public class ActivityAddedMessage
    {
        public Activity AddedActivity { get; set; }

        public ActivityAddedMessage()
        { }

        public ActivityAddedMessage(Activity activity)
        {
            this.AddedActivity = activity ?? throw new ArgumentNullException(nameof(activity));
        }
    }
}
