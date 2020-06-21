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
        private GameManager manager;
        private Guid gameId;
        private string userId;

        public GameManagerTests()
        {
            this.gameId = Guid.NewGuid();
            this.userId = Guid.NewGuid().ToString();
            this.manager = new GameManager();
            this.manager.StartGame(new Game(this.gameId, "my game", new[] { this.userId }));
        }

        [Fact]
        public void ShouldStartEmpty()
        {
            var emptyCalculator = new GameManager();
            Assert.Equal(0, emptyCalculator.Count);
        }

        [Fact]
        public void ShouldThrowOnInvalidGameId()
        {
            Guid invalidGameId = Guid.NewGuid();
            Assert.ThrowsAny<ArgumentException>(() => manager.GetGameResult(invalidGameId));
        }

        [Fact]
        public void ShouldThrowWhenNullGameWasPassed()
        {
            Assert.ThrowsAny<ArgumentNullException>(() => manager.StartGame(null));
        }

        [Fact]
        public void ShouldThrowWhenTryingToCreateSameGameMultipleTimes()
        {
            Assert.ThrowsAny<ArgumentException>(() => manager.StartGame(new Game(this.gameId, "title", new[] { this.userId })));
        }

        [Fact]
        public void ShouldGetResultForKnownGame()
        {
            manager.AddUserScore(gameId, userId, 1);

            var result = manager.GetGameResult(gameId);
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
            Assert.Equal(1, manager.AddUserScore(gameId, userId, 1));
            Assert.Equal(2, manager.AddUserScore(gameId, userId, 1));
            Assert.Equal(9, manager.AddUserScore(gameId, userId, 7));
        }

        [Fact]
        public void ShouldCalculateScoreCorrentlyAfterMultipleIncrementsForSingleUser()
        {
            manager.AddUserScore(gameId, userId, 1);
            manager.AddUserScore(gameId, userId, 3);
            manager.AddUserScore(gameId, userId, 2);

            var result = manager.GetGameResult(gameId);

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

            manager.AddUserScore(gameId, userId, 1);
            manager.AddUserScore(gameId, userId2, 4);
            manager.AddUserScore(gameId, userId3, 1);

            manager.AddUserScore(gameId, userId2, 1);
            manager.AddUserScore(gameId, userId, 3);
            manager.AddUserScore(gameId, userId3, 1);

            manager.AddUserScore(gameId, userId3, 7);
            manager.AddUserScore(gameId, userId, 2);
            manager.AddUserScore(gameId, userId2, 3);

            var result = manager.GetGameResult(gameId);

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

            manager.AddUserScore(gameId, userId, 1);
            manager.AddUserScore(gameId, userId2, 2);
            manager.AddUserScore(gameId, userId3, 3);

            var winner = manager.FindWinner(gameId);
            Assert.NotNull(winner);
            Assert.Equal(userId3, winner.UserId);
            Assert.Equal(3, winner.Score);
        }

        [Fact]
        public void ShouldReturnNoWinnerIfNoScoresAreAvailable()
        {
            var winner = manager.FindWinner(gameId);
            Assert.Null(winner);
        }
    }
}
