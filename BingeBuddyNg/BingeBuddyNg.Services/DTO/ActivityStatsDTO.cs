using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.DTO
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
