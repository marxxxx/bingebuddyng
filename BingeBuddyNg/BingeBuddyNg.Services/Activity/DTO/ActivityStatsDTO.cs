using System;
using BingeBuddyNg.Core.Statistics.DTO;

namespace BingeBuddyNg.Core.Activity.DTO
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