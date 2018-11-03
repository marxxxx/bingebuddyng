using System;

namespace BingeBuddyNg.Shared
{
    public static class Constants
    {
        public static class QueueNames
        {
            public const string ProfileUpdate = "profile-update";
            public const string ActivityAdded = "activity-added";
            public const string ReactionAdded = "reaction-added";
        }

        public static class Urls
        {
            public const string FriendRequestNotificationIconUrl = "https://bingebuddystorage.z6.web.core.windows.net/favicon.ico";
            public const string FriendRequestApplicationUrl = "https://bingebuddy.azureedge.net/friendrequests";
        }
    }
}
