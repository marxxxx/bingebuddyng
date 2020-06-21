using System;
using System.Collections.Generic;

namespace BingeBuddyNg.Core.DrinkEvent.DTO
{
    public class DrinkEventDTO
    {
        public DateTime StartUtc { get; set; }

        public DateTime EndUtc { get; set; }

        public IEnumerable<string> ScoringUserIds { get; set; }
    }
}
