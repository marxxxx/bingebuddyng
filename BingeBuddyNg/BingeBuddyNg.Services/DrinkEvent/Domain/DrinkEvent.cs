using System;
using System.Collections.Generic;

namespace BingeBuddyNg.Core.DrinkEvent.Domain
{
    public class DrinkEvent
    {
        private List<string> _scoringUserIds = new List<string>();

        public IReadOnlyList<string> ScoringUserIds => _scoringUserIds.AsReadOnly();

        public DateTime StartUtc { get; }

        public DateTime EndUtc { get; }

        public DrinkEvent(DateTime startUtc, DateTime endUtc, IEnumerable<string> scoringUserIds = null)
        {
            this.StartUtc = startUtc;
            this.EndUtc = endUtc;

            if (scoringUserIds != null)
            {
                foreach (var userId in scoringUserIds)
                {
                    this.AddScoringUserId(userId);
                }
            }
        }

        public bool AddScoringUserId(string userId)
        {
            bool isAdded = false;
            if (this._scoringUserIds.Contains(userId) == false)
            {
                this._scoringUserIds.Add(userId);
                isAdded = true;
            }

            return isAdded;
        }
    }
}