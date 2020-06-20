using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace BingeBuddyNg.Core.Statistics
{
    public class UserStatisticHistoryTableEntity : TableEntity
    {
        public DateTime CalculationTimestamp { get; set; }
        public double CurrentAlcLevel { get; set; }

        public UserStatisticHistoryTableEntity()
        {

        }

        public UserStatisticHistoryTableEntity(string userId, DateTime timestamp, double currentAlcLevel)
            :base(userId, timestamp.ToString("yyyyMMddHHmm"))
        {
            this.CalculationTimestamp = timestamp;
            this.CurrentAlcLevel = currentAlcLevel;
        }
    }
}
