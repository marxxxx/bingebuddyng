using BingeBuddyNg.Services.Game;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Shared;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock
                .Setup(s => s.GetUsersAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync((IEnumerable<string> _userIds) =>
                    _userIds.Select(u => new User() { Id = u, Name = "username", PushInfo = new PushInfo("sub", "auth", "p256dh") }).ToList());

            var gameNotificationService = new GameEndNotificationService(
                gameManager,
                notificationServiceMock.Object,
                userRepositoryMock.Object,
                new Mock<ITranslationService>().Object);

            await gameNotificationService.StartAsync(CancellationToken.None);

            gameManager.StartGame(new Game(gameId, "my game", new string[] { playerOne, playerTwo }, TimeSpan.FromSeconds(1)));

            gameManager.AddUserScore(gameId, playerOne, 1);
            gameManager.AddUserScore(gameId, playerTwo, 2);

            await Task.Delay(2000);

            notificationServiceMock.Verify(s =>
                s.SendSignalRMessageAsync(
                    It.IsAny<IEnumerable<string>>(),
                    Constants.SignalR.NotificationHubName,
                    HubMethodNames.GameEnded,
                    It.IsAny<GameEndedMessage>()));

            notificationServiceMock.Verify(s =>
                 s.SendWebPushMessage(
                     It.IsAny<IEnumerable<PushInfo>>(),
                    It.IsAny<NotificationMessage>()));

            await gameNotificationService.StopAsync(CancellationToken.None);
        }
    }
}
