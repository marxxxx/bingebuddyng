using BingeBuddyNg.Core.Statistics.DTO;
using BingeBuddyNg.Services.Statistics;
using System;

namespace BingeBuddyNg.Services.Activity
{
    public class ActivityStatsDTO
    {
        public ActivityStatsDTO(ActivityDTO activity, UserStatisticsDTO userStats)
        {
            Activity = activity ?? throw new ArgumentNullException(nameof(activity));
            UserStats = userStats ?? throw new ArgumentNullException(nameof(userStats));
        }

        public ActivityDTO Activity { get; set; }
        public UserStatisticsDTO UserStats { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Activity)}={Activity}, {nameof(UserStats)}={UserStats}}}";
        }
    }
}
