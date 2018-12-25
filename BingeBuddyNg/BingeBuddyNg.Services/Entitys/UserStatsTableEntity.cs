using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Entitys
{
    public class UserStatsTableEntity : TableEntity
    {
        public double CurrentAlcoholization { get; set; }
        public int CurrentNightDrinks { get; set; }
        public int? TotalDrinksLastMonth { get; set; }
        public int? Score { get; set; }

        public UserStatsTableEntity()
        { }

        public UserStatsTableEntity(string partitionKey, string rowKey, double currentAlcoholization, 
            int currentNightDrinks, int? score, int? totalDrinksLastMonth = null)
            :base(partitionKey, rowKey)
        {
            this.CurrentAlcoholization = currentAlcoholization;
            this.CurrentNightDrinks = currentNightDrinks;
            this.Score = score;
            this.TotalDrinksLastMonth = totalDrinksLastMonth;
        }
    }
}
