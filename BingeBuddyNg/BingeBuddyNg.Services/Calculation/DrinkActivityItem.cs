using System;

namespace BingeBuddyNg.Core.Calculation
{
    public class DrinkActivityItem
    {
        public DateTime Timestamp { get; set; }
        public double AlcPrc { get; set; }
        public double VolMl { get; set; }

        public DrinkActivityItem(DateTime timestamp)
        {
            this.Timestamp = timestamp;
        }

        public DrinkActivityItem(DateTime timestamp, double alcPrc, double volMl)
        {
            this.Timestamp = timestamp;
            this.AlcPrc = alcPrc;
            this.VolMl = volMl;
        }

        public override string ToString()
        {
            return $"{{{nameof(Timestamp)}={Timestamp}, {nameof(AlcPrc)}={AlcPrc}, {nameof(VolMl)}={VolMl}}}";
        }
    }
}
