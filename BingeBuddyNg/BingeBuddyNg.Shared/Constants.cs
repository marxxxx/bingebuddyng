using System;

namespace BingeBuddyNg.Shared
{
    public static class Constants
    {
        public const string ApplicationName = "BingeBuddy";

        public const string DefaultLanguage = "de";

        public static class ContainerNames
        {
            public const string ProfileImages = "profileimg";
        }

        public static class QueueNames
        {
            public const string ActivityAdded = "activity-added";
            public const string ReactionAdded = "reaction-added";
            public const string UserRenamed = "user-renamed";
            public const string DeleteUser = "delete-user";
            public const string DeleteActivity = "delete-activity";
        }

        public static class Urls
        {
            public const string ApplicationIconUrl = "https://bingebuddystorage.z6.web.core.windows.net/favicon.ico";
            public const string FriendRequestApplicationUrl = "https://bingebuddy.azureedge.net/users/friendrequests";
            public const string ApplicationUrl = "https://bingebuddy.azureedge.net";
        }

        public static class Scores
        {
            public const int FriendInvitation = 50;
            public const int StandardDrinkAction = 30;
        }

        public static class SignalR
        {
            public const string NotificationHubName = "notification";

            public const string ActivityReceivedMethodName = "activity-received";
        }
    }
}
