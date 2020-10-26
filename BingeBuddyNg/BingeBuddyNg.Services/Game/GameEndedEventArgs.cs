using System;

namespace BingeBuddyNg.Core.Game
{
    public class GameEndedEventArgs : EventArgs
    {
        public GameEndedEventArgs(Domain.Game game, string winnerUserId)
        {
            Game = game ?? throw new ArgumentNullException(nameof(game));
            WinnerUserId = winnerUserId;
        }

        public Domain.Game Game { get; }
        public string WinnerUserId { get; }
    }
}