using System;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;

namespace BingeBuddyNg.Services.Activity
{
    public class ActivityStatsDTO
    {
        public ActivityStatsDTO(ActivityDTO activity, UserStatistics userStats)
        {
            Activity = activity ?? throw new ArgumentNullException(nameof(activity));
            UserStats = userStats ?? throw new ArgumentNullException(nameof(userStats));
        }

        public ActivityDTO Activity { get; set; }
        public UserStatistics UserStats { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Activity)}={Activity}, {nameof(UserStats)}={UserStats}}}";
        }
    }
}
