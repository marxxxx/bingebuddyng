using System;
using BingeBuddyNg.Services.Game.Queries;
using BingeBuddyNg.Services.User;

namespace BingeBuddyNg.Services.Activity.Domain
{
    public class GameResultActivity : Activity
    {
        public GameDTO GameInfo { get; }

        private GameResultActivity(string id, ActivityType type, DateTime timestamp, Location location, GameDTO game , UserInfo winner) 
            : base(id, type, timestamp, location, winner.UserId, winner.UserName)
        {
            this.GameInfo = game ?? throw new ArgumentNullException(nameof(game));
        }

        public static GameResultActivity Create(GameDTO game, UserInfo winner)
        {
            var timestamp = DateTime.Now;
            var id = ActivityId.Create(timestamp, winner.UserId);

            var activity = new GameResultActivity(id.Value, ActivityType.GameResult, timestamp, Location.Nowhere, game, winner);

            return activity;
        }

        public static GameResultActivity Create(string id, DateTime timestamp, GameDTO game, UserInfo winner)
        {
            var activity = new GameResultActivity(id, ActivityType.GameResult, timestamp, Location.Nowhere, game, winner);

            return activity;
        }
    }
}
