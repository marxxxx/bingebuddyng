using System;
using BingeBuddyNg.Services.Activity.Domain.Events;
using BingeBuddyNg.Services.Game.Queries;

namespace BingeBuddyNg.Services.Activity.Domain
{
    public class GameActivity : Activity
    {
        public GameDTO GameInfo { get; private set; }

        private GameActivity(string id, ActivityType type, DateTime timestamp, Location location, string userId, string userName, GameDTO game)
            : base(id, type, timestamp, location, userId, userName)
        {
            this.GameInfo = game ?? throw new ArgumentNullException(nameof(game));
        }

        public static GameActivity Create(string id, ActivityType type, DateTime timestamp, Location location, string userId, string userName, GameDTO game)
        {
            var activity = new GameActivity(id, type, timestamp, location, userId, userName, game);
            return activity;
        }
    }
}
