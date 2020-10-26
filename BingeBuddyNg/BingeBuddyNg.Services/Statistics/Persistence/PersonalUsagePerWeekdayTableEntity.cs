using Microsoft.WindowsAzure.Storage.Table;

namespace BingeBuddyNg.Core.Statistics
{
    public class PersonalUsagePerWeekdayTableEntity : TableEntity
    {
        public string WeekDay { get; set; }
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