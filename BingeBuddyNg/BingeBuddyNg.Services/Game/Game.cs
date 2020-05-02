using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BingeBuddyNg.Services.Game
{
    public class Game
    {
        public Game(Guid id, string title, IEnumerable<string> playerUserIds)
        {
            this.Id = id;
            this.Title = title;
            this.PlayerUserIds = playerUserIds ?? throw new ArgumentNullException(nameof(playerUserIds));
            this.Scores = new ConcurrentDictionary<string, int>();
        }

        public Guid Id { get; }

        public string Title { get; }

        public IEnumerable<string> PlayerUserIds { get; set; }

        public ConcurrentDictionary<string, int> Scores { get; }

        public int IncrementScore(string userId, int count)
        {
            int newScore = count;
            this.Scores.AddOrUpdate(userId, count, (_userId, _currentScore) => newScore = _currentScore + count);
            return newScore;
        }
    }
}
