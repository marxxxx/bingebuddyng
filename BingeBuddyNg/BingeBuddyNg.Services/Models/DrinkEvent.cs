using System;
using System.Collections.Generic;

namespace BingeBuddyNg.Services.Models
{
    public class DrinkEvent
    {
        public DrinkEvent()
        {
        }

        public DrinkEvent(DateTime startUtc, DateTime endUtc)
        {
            StartUtc = startUtc;
            EndUtc = endUtc;
        }

        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }

        public List<string> ScoringUserIds { get; } = new List<string>();

        public bool AddScoringUserId(string userId)
        {
            bool isAdded = false;
            if (this.ScoringUserIds.Contains(userId) == false)
            {
                this.ScoringUserIds.Add(userId);
                isAdded = true;
            }

            return isAdded;

        }
        
    }
}
