using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Activity.Persistence;
using BingeBuddyNg.Services.Game;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.User.Persistence;
using BingeBuddyNg.Shared;
using Moq;
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

            GameEndNotificationService gameNotificationService =             
                SetupGameEndNotificationService(out var gameManager, out var notificationServiceMock, out var _);

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
                    It.IsAny<WebPushNotificationMessage>()));

            await gameNotificationService.StopAsync(CancellationToken.None);
        }

        [Fact]
        public async Task ShouldWriteEndResultToActivitiesAfterGameEnded()
        {
            Guid gameId = Guid.NewGuid();
            string playerOne = Guid.NewGuid().ToString();
            string playerTwo = Guid.NewGuid().ToString();

            GameEndNotificationService gameNotificationService =
                SetupGameEndNotificationService(out var gameManager, out var _, out var activityRepository);

            await gameNotificationService.StartAsync(CancellationToken.None);

            gameManager.StartGame(new Game(gameId, "my game", new string[] { playerOne, playerTwo }, TimeSpan.FromSeconds(1)));

            gameManager.AddUserScore(gameId, playerOne, 1);
            gameManager.AddUserScore(gameId, playerTwo, 2);

            await Task.Delay(2000);

            activityRepository.Verify(a => a.AddActivityAsync(It.Is<ActivityEntity>(a => a.ActivityType == ActivityType.GameResult && a.GameInfo != null && a.GameInfo.Id == gameId)));
        }

        private static GameEndNotificationService SetupGameEndNotificationService(
            out GameManager gameManager,
            out Mock<INotificationService> notificationServiceMock,
            out Mock<IActivityRepository> activityRepositoryMock)
        {
            gameManager = new GameManager();
            notificationServiceMock = new Mock<INotificationService>();
            var userRepositoryMock = new Mock<IUserRepository>();
            activityRepositoryMock = new Mock<IActivityRepository>();

            activityRepositoryMock
                .Setup(a => a.AddActivityAsync(It.IsAny<ActivityEntity>()))
                .ReturnsAsync((ActivityEntity _a) => _a);

            userRepositoryMock
                .Setup(s => s.GetUsersAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync((IEnumerable<string> _userIds) =>
                    _userIds.Select(u => new UserEntity() { Id = u, Name = "username", PushInfo = new PushInfo("sub", "auth", "p256dh") }).ToList());

            var gameNotificationService = new GameEndNotificationService(
                gameManager,
                notificationServiceMock.Object,
                userRepositoryMock.Object,
                new Mock<ITranslationService>().Object,
                activityRepositoryMock.Object);

            return gameNotificationService;
        }
    }
}
