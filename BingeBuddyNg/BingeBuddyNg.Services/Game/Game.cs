﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BingeBuddyNg.Services.Game
{
    public class Game
    {
        public Game(Guid id, IEnumerable<Guid> playerUserIds)
        {
            this.Id = id;
            this.PlayerUserIds = playerUserIds ?? throw new ArgumentNullException(nameof(playerUserIds));
            this.Scores = new ConcurrentDictionary<Guid, int>();
        }

        public Guid Id { get; }

        public IEnumerable<Guid> PlayerUserIds { get; set; }

        public ConcurrentDictionary<Guid, int> Scores { get; }

        public int IncrementScore(Guid userId, int count)
        {
            int newScore = count;
            this.Scores.AddOrUpdate(userId, count, (_userId, _currentScore) => newScore = _currentScore + count);
            return newScore;
        }
    }
}
