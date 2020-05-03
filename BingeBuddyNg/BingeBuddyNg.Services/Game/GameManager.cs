using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BingeBuddyNg.Services.Game
{
    public class GameManager : IGameManager
    {
        public ConcurrentDictionary<Guid, Game> Games { get; } = new ConcurrentDictionary<Guid, Game>();

        public event EventHandler<GameEndedEventArgs> GameEnded;

        public void StartGame(Game game)
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
            var timer = new Timer(OnGameElapsed, game , game.Duration, TimeSpan.FromMilliseconds(-1));
            game.Timer = timer;
        }

        private void OnGameElapsed(object state)
        {
            var game = (Game)state;
            var winner = FindWinner(game.Id);

            this.GameEnded?.Invoke(this, new GameEndedEventArgs(game, winner?.UserId));
            if(game.Timer != null)
            {
                game.Timer.Dispose();
                game.Timer = null;
            }            
        }

        public Game GetGame(Guid gameId)
        {
            if(!this.Games.TryGetValue(gameId, out Game game))
            {
                throw new ArgumentException($"Game {gameId} not found!");
            }

            return game;
        }

        public int AddUserScore(Guid gameId, string userId, int score)
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

        public UserScore FindWinner(Guid gameId)
        {
            if (!this.Games.TryGetValue(gameId, out Game game))
            {
                throw new ArgumentException($"Game with Id {gameId} not found!");
            }

            if(game.Scores.Count == 0)
            {
                return null;
            }

            var orderedResult = game.Scores.OrderByDescending(s => s.Value);
            var winner = orderedResult.First();
            return new UserScore(winner.Key, winner.Value);
        }
    }
}
