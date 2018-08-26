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

        public UserStatsTableEntity()
        { }

        public UserStatsTableEntity(string partitionKey, string rowKey, double currentAlcoholization, int currentNightDrinks)
            :base(partitionKey, rowKey)
        {
            this.CurrentAlcoholization = currentAlcoholization;
            this.CurrentNightDrinks = currentNightDrinks;
        }
    }
}
