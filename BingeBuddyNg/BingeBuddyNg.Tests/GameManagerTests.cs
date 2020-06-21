using BingeBuddyNg.Core.Game;
using BingeBuddyNg.Core.Game.Domain;
using BingeBuddyNg.Services.Game;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BingeBuddyNg.Tests
{
    public class GameManagerTests
    {
        private GameRepository manager;
        private Guid gameId;
        private string userId;

        public GameManagerTests()
        {
            this.gameId = Guid.NewGuid();
            this.userId = Guid.NewGuid().ToString();
            this.manager = new GameRepository();
            var game = this.manager.Create("my game", new[] { this.userId });
            this.gameId = game.Id;
        }

        [Fact]
        public void ShouldStartEmpty()
        {
            var emptyCalculator = new GameRepository();
            Assert.Equal(0, emptyCalculator.Count);
        }

        [Fact]
        public void ShouldThrowOnInvalidGameId()
        {
            Guid invalidGameId = Guid.NewGuid();
            Assert.ThrowsAny<ArgumentException>(() => manager.Get(invalidGameId));
        }

        [Fact]
        public void ShouldThrowWhenNullTitleWasPassed()
        {
            Assert.ThrowsAny<ArgumentNullException>(() => manager.Create(null, new[] { Guid.NewGuid().ToString() }));
        }

        [Fact]
        public void ShouldGetResultForKnownGame()
        {
            manager.Get(gameId).IncrementScore(userId, 1);

            var result = manager.Get(gameId).GetResult();
            Assert.NotNull(result);
            Assert.Collection(result,
                (r) =>
                {
                    Assert.Equal(userId, r.UserId);
                    Assert.Equal(1, r.Score);
                });
        }

        [Fact]
        public void ShouldReturnCurrentScoreAfterIncrement()
        {
            var game = manager.Get(gameId);
            Assert.Equal(1, game.IncrementScore(userId, 1));
            Assert.Equal(2, game.IncrementScore(userId, 1));
            Assert.Equal(9, game.IncrementScore(userId, 7));
        }

        [Fact]
        public void ShouldCalculateScoreCorrentlyAfterMultipleIncrementsForSingleUser()
        {
            var game = manager.Get(gameId);
            game.IncrementScore(userId, 1);
            game.IncrementScore(userId, 3);
            game.IncrementScore(userId, 2);

            var result = manager.Get(gameId).GetResult();

            Assert.NotNull(result);
            Assert.Collection(result, (r) =>
            {
                Assert.Equal(userId, r.UserId);
                Assert.Equal(6, r.Score);
            });
        }

        [Fact]
        public void ShouldCalculateScoreCorrentlyAfterMultipleIncrementsForMultipleUsers()
        {
            string userId2 = Guid.NewGuid().ToString();
            string userId3 = Guid.NewGuid().ToString();

            var game = manager.Get(gameId);

            game.IncrementScore(userId, 1);
            game.IncrementScore(userId2, 4);
            game.IncrementScore(userId3, 1);

            game.IncrementScore(userId2, 1);
            game.IncrementScore(userId, 3);
            game.IncrementScore(userId3, 1);

            game.IncrementScore(userId3, 7);
            game.IncrementScore(userId, 2);
            game.IncrementScore(userId2, 3);

            var result = game.GetResult();

            Assert.NotNull(result);
            Assert.Contains(result, r => r.UserId == userId && r.Score == 6);
            Assert.Contains(result, r => r.UserId == userId2 && r.Score == 8);
            Assert.Contains(result, r => r.UserId == userId3 && r.Score == 9);
        }

        [Fact]
        public void ShouldDetermineWinner()
        {
            string userId2 = Guid.NewGuid().ToString();
            string userId3 = Guid.NewGuid().ToString();

            var game = manager.Get(gameId);

            game.IncrementScore(userId, 1);
            game.IncrementScore(userId2, 2);
            game.IncrementScore(userId3, 3);

            var winner = game.FindWinner();
            Assert.NotNull(winner);
            Assert.Equal(userId3, winner.UserId);
            Assert.Equal(3, winner.Score);
        }

        [Fact]
        public void ShouldReturnNoWinnerIfNoScoresAreAvailable()
        {
            var winner = manager.Get(gameId).FindWinner();
            Assert.Null(winner);
        }
    }
}
