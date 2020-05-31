using System;

namespace BingeBuddyNg.Services.Activity
{
    public class ActivityAggregationDTO
    {
        public DateTime Day { get; set; }
        public int Count { get; set; }
        public int CountBeer { get; set; }
        public int CountWine { get; set; }
        public int CountShots { get; set; }
        public int CountAnti { get; set; }
        public int CountAlc { get; set; }

        public ActivityAggregationDTO()
        { }

        public ActivityAggregationDTO(DateTime day)
        {
            this.Day = day;
        }

        public ActivityAggregationDTO(DateTime day, int count, int countBeer, int countWine,
            int countShots, int countAnti, int countAlc)
        {
            this.Day = day;
            this.Count = count;
            this.CountBeer = countBeer;
            this.CountWine = countWine;
            this.CountShots = countShots;
            this.CountAnti = countAnti;
            this.CountAlc = countAlc;
        }

        public override string ToString()
        {
            return $"{{{nameof(Day)}={Day}, {nameof(Count)}={Count}, {nameof(CountBeer)}={CountBeer}, {nameof(CountWine)}={CountWine}, {nameof(CountShots)}={CountShots}, {nameof(CountAnti)}={CountAnti}, {nameof(CountAlc)}={CountAlc}}}";
        }
    }
}
