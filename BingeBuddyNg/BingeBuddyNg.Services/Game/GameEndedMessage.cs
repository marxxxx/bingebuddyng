using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Game
{
    public class GameEndedMessage
    {
        public GameEndedMessage(Guid gameId, string winnerUserId)
        {
            GameId = gameId;
            WinnerUserId = winnerUserId ?? throw new ArgumentNullException(nameof(winnerUserId));
        }

        public Guid GameId { get; set; }

        public string WinnerUserId { get; set; }
    }
}
