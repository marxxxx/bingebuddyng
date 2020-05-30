﻿using System;
using BingeBuddyNg.Services.Activity;

namespace BingeBuddyNg.Functions.Services.Notifications
{
    public class ActivityNotification : NotificationBase
    {
        public Activity Activity { get; }

        public ActivityNotification(string userId, Activity activity)
            :base(userId)
        {
            this.Activity = activity ?? throw new ArgumentNullException(nameof(activity));
        }        
    }
}
