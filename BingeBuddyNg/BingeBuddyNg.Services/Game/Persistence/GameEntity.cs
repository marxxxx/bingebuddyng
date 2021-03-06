﻿using System;
using System.Collections.Generic;
using BingeBuddyNg.Core.Game.Domain;

namespace BingeBuddyNg.Core.Game.Persistence
{
    public class GameEntity
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public IEnumerable<UserScoreInfo> UserScores { get; set; }

        public GameStatus Status { get; set; }

        public string WinnerUserId { get; set; }
    }
}