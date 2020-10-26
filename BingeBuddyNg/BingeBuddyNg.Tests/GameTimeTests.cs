using System;
using System.Threading;
using BingeBuddyNg.Core.Game;
using BingeBuddyNg.Core.Game.Domain;
using Xunit;

namespace BingeBuddyNg.Tests
{
    public class GameTimeTests
    {
        private GameRepository manager;
        private Game game;
        private string userId;
        private string friendUserId;

        public GameTimeTests()
        {
            this.userId = Guid.NewGuid().ToString();
            this.friendUserId = Guid.NewGuid().ToString();
            this.manager = new GameRepository();
            this.game = this.manager.Create("my game", new[] { this.friendUserId }, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void ShouldRaiseEndGameEventAfterGameTimeElapsed()
        {
            AutoResetEvent endedEvent = new AutoResetEvent(false);
            GameEndedEventArgs args = null;
            this.game.GameEnded += (o, e) =>
            {
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    args = e;
                    endedEvent.Set();
                });
            };
            game.Start();

            game.IncrementScore(userId, 1);
            game.IncrementScore(friendUserId, 2);
            game.IncrementScore(userId, 3);

            endedEvent.WaitOne(1500);

            Assert.NotNull(args);
            Assert.Equal(userId, args.WinnerUserId);
            Assert.Equal(game.Id, args.Game.Id);
        }
    }
}