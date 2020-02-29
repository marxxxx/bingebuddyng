using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Statistics
{
    public class PersonalUsagePerWeekdayTableEntity : TableEntity
    {
        public string weekDay { get; set; }
        public string ActivityCount { get; set; }
        public string AvgCount { get; set; }
        public string MaxCount { get; set; }
        public string MinCount { get; set; }
        public string MedianActivityCount { get; set; }
        public string MedianMaxAlcLevel { get; set; }
        public string Percentage { get; set; }
        public string Probability { get; set; }
    }
}   
