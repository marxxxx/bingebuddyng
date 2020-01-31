using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Statistics
{
    public class PersonalUsagePerWeekdayTableEntity : TableEntity
    {
        public string weekDay { get; set; }
        public int ActivityCount { get; set; }
        public double AvgCount { get; set; }
        public int MaxCount { get; set; }
        public int MinCount { get; set; }
        public double MedialActivityCount { get; set; }
        public double MedianMaxAlcLevel { get; set; }
        public double Percentage { get; set; }
    }
}   
