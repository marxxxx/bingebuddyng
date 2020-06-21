using System;
using System.Threading;
using BingeBuddyNg.Core.Game;
using BingeBuddyNg.Core.Game.Domain;
using Xunit;

namespace BingeBuddyNg.Tests
{
    public class GameManagerTimeTests
    {
        private GameManager manager;
        private Guid gameId;
        private string userId;
        private string friendUserId;

        public GameManagerTimeTests()
        {
            this.gameId = Guid.NewGuid();
            this.userId = Guid.NewGuid().ToString();
            this.friendUserId = Guid.NewGuid().ToString();
            this.manager = new GameManager();
            this.manager.StartGame(new Game(this.gameId, "my game", new[] { this.friendUserId }, TimeSpan.FromSeconds(1)));
        }

        [Fact]
        public void ShouldRaiseEndGameEventAfterGameTimeElapsed()
        {
            AutoResetEvent endedEvent = new AutoResetEvent(false);
            GameEndedEventArgs args = null;
            manager.GameEnded += (o, e) =>
            {
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    args = e;
                    endedEvent.Set();
                });
            };

            manager.AddUserScore(gameId, userId, 1);
            manager.AddUserScore(gameId, friendUserId, 2);
            manager.AddUserScore(gameId, userId, 3);

            endedEvent.WaitOne(1500);

            Assert.NotNull(args);
            Assert.Equal(userId, args.WinnerUserId);
            Assert.Equal(gameId, args.Game.Id);
        }
    }
}
