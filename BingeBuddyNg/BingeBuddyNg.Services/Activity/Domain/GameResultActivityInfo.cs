using System;
using BingeBuddyNg.Services.Game.Persistence;

namespace BingeBuddyNg.Core.Activity.Domain
{
    public class GameResultActivityInfo
    {
        // TODO: This should be a domain object
        public GameEntity GameInfo { get; }

        public GameResultActivityInfo(GameEntity game) 
        {
            this.GameInfo = game ?? throw new ArgumentNullException(nameof(game));
        }
    }
}
