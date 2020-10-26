using System;
using BingeBuddyNg.Core.Game.Persistence;

namespace BingeBuddyNg.Core.Activity.Domain
{
    public class GameResultActivityInfo
    {
        public GameEntity GameInfo { get; }

        public GameResultActivityInfo(GameEntity game)
        {
            this.GameInfo = game ?? throw new ArgumentNullException(nameof(game));
        }
    }
}