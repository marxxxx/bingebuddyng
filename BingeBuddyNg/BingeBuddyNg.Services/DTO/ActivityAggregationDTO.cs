using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.DTO
{
    public class ActivityAggregationDTO
    {
        public DateTime Day { get; set; }
        public int Count { get; set; }

        public ActivityAggregationDTO()
        { }

        public ActivityAggregationDTO(DateTime day, int count)
        {
            this.Day = day;
            this.Count = count;
        }

        public override string ToString()
        {
            return $"{nameof(Day)}: [{Day}], {nameof(Count)}: [{Count}]";
        }
    }
}
