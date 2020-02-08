using System;

namespace BingeBuddyNg.Services.Statistics
{
    public class PersonalUsagePerWeekdayDTO
    {
        public string WeekDay { get; set; }
        public int ActivityCount { get; set; }
        public double AvgCount { get; set; }
        public int MaxCount { get; set; }
        public int MinCount { get; set; }
        public double MedianActivityCount { get; set; }
        public double MedianMaxAlcLevel { get; set; }
        public double Percentage { get; set; }
    }
}
