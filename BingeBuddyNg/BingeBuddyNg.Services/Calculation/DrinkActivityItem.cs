using System;

namespace BingeBuddyNg.Core.Calculation
{
    public class DrinkActivityItem
    {
        public DateTime Timestamp { get; }
        public double AlcPrc { get; }
        public double VolMl { get; }

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