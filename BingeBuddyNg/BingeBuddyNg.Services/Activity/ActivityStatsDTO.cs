using System;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;

namespace BingeBuddyNg.Services.Activity
{
    public class ActivityStatsDTO
    {
        public ActivityStatsDTO(Activity activity, UserStatistics userStats)
        {
            Activity = activity ?? throw new ArgumentNullException(nameof(activity));
            UserStats = userStats ?? throw new ArgumentNullException(nameof(userStats));
        }

        public Activity Activity { get; set; }
        public UserStatistics UserStats { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Activity)}={Activity}, {nameof(UserStats)}={UserStats}}}";
        }
    }
}
