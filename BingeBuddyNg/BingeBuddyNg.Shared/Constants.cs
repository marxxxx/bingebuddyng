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
            public const string FriendStatusChanged = "friend-status-changed";
        }

        public static class TableNames
        {
            public const string Activity = "activity";
            public const string ActivityPerUser = "activityperuser";
            public const string ActivityUserFeed = "activityuserfeed";
            public const string Users = "users";
            public const string UserStats = "userstats";
            public const string UserStatsHistory = "userstatshistory";
            public const string Reports = "reports";
        }

        public static class StaticPartitionKeys
        {
            public const string User = "User";
            public const string UserStats = "UserStats";
            public const string PersonalUsagePerWeekdayReport = "personalusageperweekdayreport";
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
