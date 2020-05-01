using BingeBuddyNg.Services.Game;
using System;
using Xunit;

namespace BingeBuddyNg.Tests
{
    public class GameManagerTests
    {
        private GameManager calculator;
        private Guid gameId;
        private Guid userId;

        public GameManagerTests()
        {
            this.gameId = Guid.NewGuid();
            this.userId = Guid.NewGuid();
            this.calculator = new GameManager();
            this.calculator.CreateGame(new Game(this.gameId, "my game", new[] { this.userId }));
        }

        [Fact]
        public void ShouldStartEmpty()
        {
            var emptyCalculator = new GameManager();
            Assert.Empty(emptyCalculator.Games);
        }

        [Fact]
        public void ShouldThrowOnInvalidGameId()
        {
            Guid invalidGameId = Guid.NewGuid();
            Assert.ThrowsAny<ArgumentException>(() => calculator.GetGameResult(invalidGameId));
        }

        [Fact]
        public void ShouldThrowWhenNullGameWasPassed()
        {
            Assert.ThrowsAny<ArgumentNullException>(() => calculator.CreateGame(null));
        }

        [Fact]
        public void ShouldThrowWhenTryingToCreateSameGameMultipleTimes()
        {
            Assert.ThrowsAny<ArgumentException>(() => calculator.CreateGame(new Game(this.gameId, "title", new[] { this.userId })));
        }

        [Fact]
        public void ShouldGetResultForKnownGame()
        {
            calculator.AddUserScore(gameId, userId, 1);

            var result = calculator.GetGameResult(gameId);
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
            Assert.Equal(1, calculator.AddUserScore(gameId, userId, 1));
            Assert.Equal(2, calculator.AddUserScore(gameId, userId, 1));
            Assert.Equal(9, calculator.AddUserScore(gameId, userId, 7));
        }

        [Fact]
        public void ShouldCalculateScoreCorrentlyAfterMultipleIncrementsForSingleUser()
        {
            calculator.AddUserScore(gameId, userId, 1);
            calculator.AddUserScore(gameId, userId, 3);
            calculator.AddUserScore(gameId, userId, 2);

            var result = calculator.GetGameResult(gameId);

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
            Guid userId2 = Guid.NewGuid();
            Guid userId3 = Guid.NewGuid();

            calculator.AddUserScore(gameId, userId, 1);
            calculator.AddUserScore(gameId, userId2, 4);
            calculator.AddUserScore(gameId, userId3, 1);

            calculator.AddUserScore(gameId, userId2, 1);
            calculator.AddUserScore(gameId, userId, 3);
            calculator.AddUserScore(gameId, userId3, 1);

            calculator.AddUserScore(gameId, userId3, 7);
            calculator.AddUserScore(gameId, userId, 2);
            calculator.AddUserScore(gameId, userId2, 3);

            var result = calculator.GetGameResult(gameId);

            Assert.NotNull(result);
            Assert.Contains(result, r => r.UserId == userId && r.Score == 6);
            Assert.Contains(result, r => r.UserId == userId2 && r.Score == 8);
            Assert.Contains(result, r => r.UserId == userId3 && r.Score == 9);
        }

        [Fact]
        public void ShouldDetermineWinner()
        {
            Guid userId2 = Guid.NewGuid();
            Guid userId3 = Guid.NewGuid();

            calculator.AddUserScore(gameId, userId, 1);
            calculator.AddUserScore(gameId, userId2, 2);
            calculator.AddUserScore(gameId, userId3, 3);

            var winner = calculator.GetWinner(gameId);
            Assert.NotNull(winner);
            Assert.Equal(userId3, winner.UserId);
            Assert.Equal(3, winner.Score);
        }
    }
}
