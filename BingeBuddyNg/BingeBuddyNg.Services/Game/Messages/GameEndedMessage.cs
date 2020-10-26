using System;

namespace BingeBuddyNg.Core.Game
{
    public class GameEndedMessage
    {
        public GameEndedMessage(Guid gameId, string winnerUserId)
        {
            GameId = gameId;
            WinnerUserId = winnerUserId;
        }

        public Guid GameId { get; set; }

        public string WinnerUserId { get; set; }
    }
}