using System;

namespace BingeBuddyNg.Services.Game
{
    public class GameEndedEventArgs : EventArgs
    {
        public GameEndedEventArgs(Game game, string winnerUserId)
        {
            Game = game ?? throw new ArgumentNullException(nameof(game));
            WinnerUserId = winnerUserId;
        }

        public Game Game { get; }
        public string WinnerUserId { get; }
    }
}
