using System;
using System.Collections.Generic;
using BingeBuddyNg.Core.Game.Domain;

namespace BingeBuddyNg.Services.Game.DTO
{
    public class GameDTO
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public IEnumerable<UserScoreInfoDTO> UserScores { get; set; }

        public GameStatus Status { get; set; }

        public string WinnerUserId { get; set; }
    }
}
