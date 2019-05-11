using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Statistics
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
