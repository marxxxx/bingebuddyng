using BingeBuddyNg.Services.Game;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Shared;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BingeBuddyNg.Tests
{
    public class GameManagerNotificationTests
    {
        [Fact]
        public async Task ShouldSendNotificationsAfterGameEnded()
        {
            Guid gameId = Guid.NewGuid();
            string playerOne = Guid.NewGuid().ToString();
            string playerTwo = Guid.NewGuid().ToString();
            var gameManager = new GameManager();
            var notificationServiceMock = new Mock<INotificationService>();
            var gameNotificationService = new GameEndNotificationService(gameManager, notificationServiceMock.Object);

            await gameNotificationService.StartAsync(CancellationToken.None);

            gameManager.StartGame(new Game(gameId, "my game", new string[] { playerOne, playerTwo }, TimeSpan.FromSeconds(1)));

            notificationServiceMock.Verify(s =>
                s.SendWebPushMessage(It.IsAny<IEnumerable<PushInfo>>(),
                It.IsAny<NotificationMessage>()));

            notificationServiceMock.Verify(s =>
                s.SendSignalRMessageAsync(
                    It.IsAny<IEnumerable<string>>(), 
                    Constants.SignalR.NotificationHubName, 
                    HubMethodNames.GameEnded,
                    It.IsAny<NotificationMessage>()));

            await gameNotificationService.StopAsync(CancellationToken.None);

        }
    }
}
