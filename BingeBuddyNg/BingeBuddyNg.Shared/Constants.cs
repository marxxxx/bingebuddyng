using System;

namespace BingeBuddyNg.Shared
{
    public static class Constants
    {
        public const string DefaultLanguage = "de";

        public static class ContainerNames
        {
            public const string ProfileImages = "profileimg";
        }

        public static class QueueNames
        {
            public const string ProfileUpdate = "profile-update";
            public const string ActivityAdded = "activity-added";
            public const string ReactionAdded = "reaction-added";
        }

        public static class Urls
        {
            public const string ApplicationIconUrl = "https://bingebuddystorage.z6.web.core.windows.net/favicon.ico";
            public const string FriendRequestApplicationUrl = "https://bingebuddy.azureedge.net/friendrequests";
            public const string ApplicationUrl = "https://bingebuddy.azureedge.net";
        }

        public static class Scores
        {
            public const int FriendInvitation = 50;
            public const int StandardDrinkAction = 30;
        }
    }
}
