using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BingeBuddyNg.Services.Game
{
    public class GameManager : IGameManager
    {
        public ConcurrentDictionary<Guid, Game> Games { get; } = new ConcurrentDictionary<Guid, Game>();

        public void CreateGame(Game game)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            if (this.Games.ContainsKey(game.Id))
            {
                throw new ArgumentException($"Game {game.Id} already exists");
            }

            this.Games.AddOrUpdate(game.Id, game, (id, g) => game);
        }

        public Game GetGame(Guid gameId)
        {
            if(!this.Games.TryGetValue(gameId, out Game game))
            {
                throw new ArgumentException($"Game {gameId} not found!");
            }

            return game;
        }

        public int AddUserScore(Guid gameId, Guid userId, int score)
        {
            if (!this.Games.TryGetValue(gameId, out Game game))
            {
                throw new ArgumentException($"Game {gameId} not found!");
            }

            var newScore = game.IncrementScore(userId, score);

            return newScore;
        }

        public IReadOnlyList<UserScore> GetGameResult(Guid gameId)
        {
            if (!this.Games.TryGetValue(gameId, out Game game))
            {
                throw new ArgumentException($"Game with Id {gameId} not found!");
            }

            return game.Scores.Select(s => new UserScore(s.Key, s.Value)).ToList().AsReadOnly();
        }

        public UserScore GetWinner(Guid gameId)
        {
            if (!this.Games.TryGetValue(gameId, out Game game))
            {
                throw new ArgumentException($"Game with Id {gameId} not found!");
            }

            var orderedResult = game.Scores.OrderByDescending(s => s.Value);
            var winner = orderedResult.First();
            return new UserScore(winner.Key, winner.Value);
        }
    }
}
