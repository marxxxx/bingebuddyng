using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BingeBuddyNg.Core.Game.Domain
{
    public class Game
    {
        private Timer timer;

        public event EventHandler<GameEndedEventArgs> GameEnded;

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

        public IEnumerable<string> PlayerUserIds { get; }

        public ConcurrentDictionary<string, int> Scores { get; }

        public TimeSpan Duration { get; }

        public GameStatus Status { get; private set; }

        public UserScore Winner { get; private set; }

        public void Start()
        {
            this.timer = new Timer(OnGameElapsed, this, this.Duration, TimeSpan.FromMilliseconds(-1));
        }

        public int IncrementScore(string userId, int count)
        {
            int newScore = count;
            this.Scores.AddOrUpdate(userId, count, (_userId, _currentScore) => newScore = _currentScore + count);
            return newScore;
        }

        public IReadOnlyList<UserScore> GetResult()
        {
            return this.Scores.Select(s => new UserScore(s.Key, s.Value)).ToList().AsReadOnly();
        }

        public UserScore FindWinner()
        {
            if (this.Scores.Count == 0)
            {
                return null;
            }

            var orderedResult = this.Scores.OrderByDescending(s => s.Value);
            var winner = orderedResult.First();
            return new UserScore(winner.Key, winner.Value);
        }

        private void OnGameElapsed(object state)
        {
            var winner = FindWinner();

            this.Status = GameStatus.Ended;
            this.Winner = winner;

            this.GameEnded?.Invoke(this, new GameEndedEventArgs(this, winner?.UserId));
            if (this.timer != null)
            {
                this.timer.Dispose();
                this.timer = null;
            }
        }
    }
}