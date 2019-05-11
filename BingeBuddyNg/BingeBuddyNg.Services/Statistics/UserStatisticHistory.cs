using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Statistics
{
    public class UserStatisticHistory
    {
        public UserStatisticHistory()
        {
        }

        public UserStatisticHistory(string userId, DateTime timestamp, double currentAlcLevel)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Timestamp = timestamp;
            CurrentAlcLevel = currentAlcLevel;
        }

        public string UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public double CurrentAlcLevel { get; set; }
    }
}
