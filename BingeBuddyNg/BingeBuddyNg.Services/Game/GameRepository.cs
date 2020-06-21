using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BingeBuddyNg.Core.Game
{
    public class GameRepository
    {
        private ConcurrentDictionary<Guid, Domain.Game> Games { get; } = new ConcurrentDictionary<Guid, Domain.Game>();

        public int Count => Games.Count;

        public TimeSpan DefaultGameDuration { get; set; } = TimeSpan.FromMinutes(1);

        public Domain.Game Create(string title, IEnumerable<string> playerUserIds)
        {
            return this.Create(title, playerUserIds, DefaultGameDuration);
        }

        public Domain.Game Create(string title, IEnumerable<string> playerUserIds, TimeSpan duration)
        {
            if (title == null)
            {
                throw new ArgumentNullException(nameof(title));
            }

            if(playerUserIds == null || playerUserIds.Count() == 0)
            {
                throw new ArgumentException("Please invite players to the game!");
            }

            var game = new Domain.Game(Guid.NewGuid(), title, playerUserIds, duration);

            this.Games.AddOrUpdate(game.Id, game, (id, g) => game);

            return game;
        }

        public Domain.Game Get(Guid gameId)
        {
            if(!this.Games.TryGetValue(gameId, out Domain.Game game))
            {
                throw new ArgumentException($"Game {gameId} not found!");
            }

            return game;
        }
    }
}
