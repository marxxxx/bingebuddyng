using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.Game;
using BingeBuddyNg.Core.Game.Commands;
using BingeBuddyNg.Services.Activity.Persistence;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Shared;
using BingeBuddyNg.Tests.Helpers;
using Moq;
using Xunit;

namespace BingeBuddyNg.Tests
{
    public class GameManagerNotificationTests
    {
        [Fact]
        public async Task ShouldSendNotificationsAfterGameEnded()
        {
            string playerOne = Guid.NewGuid().ToString();
            string playerTwo = Guid.NewGuid().ToString();

            var sut = SetupStartGameCommandHandler(new[] { playerOne, playerTwo }, out var gameManager, out var notificationServiceMock, out var _);

            var result = await sut.Handle(new StartGameCommand(playerOne, "my game", new string[] { playerOne, playerTwo }), CancellationToken.None);

            gameManager.Get(result.GameId).IncrementScore(playerOne, 1);
            gameManager.Get(result.GameId).IncrementScore(playerTwo, 2);

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
        }

        [Fact]
        public async Task ShouldWriteEndResultToActivitiesAfterGameEnded()
        {
            string playerOne = Guid.NewGuid().ToString();
            string playerTwo = Guid.NewGuid().ToString();

            var sut = SetupStartGameCommandHandler(new[] { playerOne, playerTwo }, out var gameRepository, out var _, out var activityRepository);

            var result = await sut.Handle(new StartGameCommand(playerOne, "new game", new[] { playerOne, playerTwo }), CancellationToken.None);

            gameRepository.Get(result.GameId).IncrementScore(playerOne, 1);
            gameRepository.Get(result.GameId).IncrementScore(playerTwo, 2);

            await Task.Delay(2000);

            activityRepository.Verify(a => a.AddActivityAsync(It.Is<ActivityEntity>(a => a.ActivityType == ActivityType.GameResult && a.GameInfo != null && a.GameInfo.Id == result.GameId)));
        }

        private static StartGameCommandHandler SetupStartGameCommandHandler(
            IEnumerable<string> testUserIds,
            out GameRepository gameManager,
            out Mock<INotificationService> notificationServiceMock,
            out Mock<IActivityRepository> activityRepositoryMock)
        {
            gameManager = new GameRepository();
            gameManager.DefaultGameDuration = TimeSpan.FromSeconds(1);
            notificationServiceMock = new Mock<INotificationService>();

            var searchUsersQuery = SetupHelpers.SetupSearchUsersQuery(testUserIds);
            activityRepositoryMock = new Mock<IActivityRepository>();

            activityRepositoryMock
                .Setup(a => a.AddActivityAsync(It.IsAny<ActivityEntity>()))
                .ReturnsAsync((ActivityEntity _a) => _a);

            var startGameCommand = new StartGameCommandHandler(
                gameManager,
                searchUsersQuery,
                activityRepositoryMock.Object,
                notificationServiceMock.Object,
                new Mock<ITranslationService>().Object);

            return startGameCommand;
        }
    }
}
