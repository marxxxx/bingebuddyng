using System;
using System.Collections.Generic;

namespace BingeBuddyNg.Services.Game.Queries
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
