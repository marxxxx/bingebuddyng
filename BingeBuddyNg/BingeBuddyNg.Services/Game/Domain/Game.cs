using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace BingeBuddyNg.Core.Game.Domain
{
    public class Game
    {
        public Game(Guid id, string title, IEnumerable<string> playerUserIds)
            : this(id, title, playerUserIds, TimeSpan.FromMinutes(1))
        {
        }

        public Game(Guid id, string title, IEnumerable<string> playerUserIds, TimeSpan duration)
        {
            this.Id = id;
            this.Title = title;
            this.PlayerUserIds = playerUserIds ?? throw new ArgumentNullException(nameof(playerUserIds));
            this.Scores = new ConcurrentDictionary<string, int>();
            this.Duration = duration;
            this.Status = GameStatus.Running;
        }

        public Guid Id { get; }

        public string Title { get; }

        public IEnumerable<string> PlayerUserIds { get; set; }

        public ConcurrentDictionary<string, int> Scores { get; }

        public TimeSpan Duration { get; }

        public Timer Timer { get; set; }

        public GameStatus Status { get; set; }

        public UserScore Winner { get; set; }

        public int IncrementScore(string userId, int count)
        {
            int newScore = count;
            this.Scores.AddOrUpdate(userId, count, (_userId, _currentScore) => newScore = _currentScore + count);
            return newScore;
        }
    }
}
