using System;
using System.Collections.Generic;

namespace BingeBuddyNg.Core.DrinkEvent.Persistence
{
    public class DrinkEventEntity
    {
        public DateTime StartUtc { get; set; }

        public DateTime EndUtc { get; set; }

        public List<string> ScoringUserIds { get; } = new List<string>();
    }
}