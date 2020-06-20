using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.Game;
using BingeBuddyNg.Core.Game.Domain;
using BingeBuddyNg.Services.Activity.Persistence;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User.Persistence;
using BingeBuddyNg.Services.User.Queries;
using BingeBuddyNg.Shared;
using BingeBuddyNg.Tests.Helpers;
using Moq;
using Xunit;
using static BingeBuddyNg.Shared.Constants;

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
                SetupGameEndNotificationService(new[] { playerOne, playerTwo }, out var gameManager, out var notificationServiceMock, out var _);

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
                SetupGameEndNotificationService(new[] { playerOne, playerTwo }, out var gameManager, out var _, out var activityRepository);

            await gameNotificationService.StartAsync(CancellationToken.None);

            gameManager.StartGame(new Game(gameId, "my game", new string[] { playerOne, playerTwo }, TimeSpan.FromSeconds(1)));

            gameManager.AddUserScore(gameId, playerOne, 1);
            gameManager.AddUserScore(gameId, playerTwo, 2);

            await Task.Delay(2000);

            activityRepository.Verify(a => a.AddActivityAsync(It.Is<ActivityEntity>(a => a.ActivityType == ActivityType.GameResult && a.GameInfo != null && a.GameInfo.Id == gameId)));
        }

        private static GameEndNotificationService SetupGameEndNotificationService(
            IEnumerable<string> testUserIds,
            out GameManager gameManager,
            out Mock<INotificationService> notificationServiceMock,
            out Mock<IActivityRepository> activityRepositoryMock)
        {
            gameManager = new GameManager();
            notificationServiceMock = new Mock<INotificationService>();

            var searchUsersQuery = SetupHelpers.SetupSearchUsersQuery(testUserIds);
            activityRepositoryMock = new Mock<IActivityRepository>();

            activityRepositoryMock
                .Setup(a => a.AddActivityAsync(It.IsAny<ActivityEntity>()))
                .ReturnsAsync((ActivityEntity _a) => _a);

            var gameNotificationService = new GameEndNotificationService(
                gameManager,
                notificationServiceMock.Object,
                searchUsersQuery,
                new Mock<ITranslationService>().Object,
                activityRepositoryMock.Object);

            return gameNotificationService;
        }
    }
}
