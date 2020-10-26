using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Game;
using BingeBuddyNg.Core.Game.Commands;
using BingeBuddyNg.Core.Game.DTO;
using BingeBuddyNg.Core.Game.Queries;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Tests.Helpers;
using Moq;
using Xunit;

namespace BingeBuddyNg.Tests
{
    public class GameTests
    {
        [Fact]
        public async Task ShouldCreateGameAndSendNotificationsWhenGameWasStarted()
        {
            // Arrange
            string myUserId = Guid.NewGuid().ToString();
            string[] friendUserIds = new [] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            string gameTitle = "My Game";
            var notificationServiceMock = new Mock<INotificationService>();
            var manager = new GameRepository();

            var searchUsersQuery = SetupHelpers.SetupUserRepository(friendUserIds.Concat(new[] { myUserId }));
            
            var command = new StartGameCommand(myUserId, gameTitle, friendUserIds);
            var handler = new StartGameCommandHandler(manager, searchUsersQuery, new Mock<IActivityRepository>().Object, notificationServiceMock.Object, new Mock<ITranslationService>().Object);

            // Act
            StartGameResultDTO result = await handler.Handle(command, CancellationToken.None);

            // Assert
            var game = manager.Get(result.GameId);
            Assert.Equal(result.GameId, game.Id);

            Assert.NotNull(result);
            Assert.NotEqual(default(Guid), result.GameId);
            notificationServiceMock.Verify(s =>
                s.SendSignalRMessageAsync(
                    It.Is<IReadOnlyList<string>>(u => AreEqual(u, friendUserIds)), 
                    Shared.Constants.SignalR.NotificationHubName, 
                    HubMethodNames.GameStarted,
                    It.Is<GameStartedMessage>( m => m.GameId == result.GameId && m.Title ==  gameTitle && AreEqual<string>(friendUserIds, m.UserIds))), Times.Once);

            notificationServiceMock.Verify(s =>
               s.SendWebPushMessage(It.IsAny<IEnumerable<PushInfo>>(), It.Is<WebPushNotificationMessage>(m => m.data.url.Contains(game.Id.ToString()))), Times.Once);
        }

        [Fact]
        public async Task ShouldSendMessageWithIncrementedScoreWhenGameEventWasSent()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            string[] friendUserIds = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            string[] friendUserIdsAsString = friendUserIds.Select(f => f.ToString()).ToArray();
            var notificationServiceMock = new Mock<INotificationService>();
            var manager = new GameRepository();
            var game = manager.Create("my game", friendUserIds);

            var command = new AddGameEventCommand(game.Id, userId, 5);
            var handler = new AddGameEventCommandHandler(notificationServiceMock.Object, manager);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            notificationServiceMock.Verify(s =>
                s.SendSignalRMessageAsync(
                    It.Is<IReadOnlyList<string>>(u => AreEqual(u, friendUserIdsAsString)),
                    Shared.Constants.SignalR.NotificationHubName,
                    HubMethodNames.GameUpdateReceived,
                    It.Is<GameUpdateReceivedMessage>(m => m.GameId == game.Id && m.UserId == userId && m.CurrentScore == 5)), Times.Once);

            var secondCommand = new AddGameEventCommand(game.Id, userId, 3);
            await handler.Handle(secondCommand, CancellationToken.None);

            notificationServiceMock.Verify(s =>
                s.SendSignalRMessageAsync(
                    It.Is<IReadOnlyList<string>>(u => AreEqual(u, friendUserIdsAsString)),
                    Shared.Constants.SignalR.NotificationHubName,
                    HubMethodNames.GameUpdateReceived,
                    It.Is<GameUpdateReceivedMessage>(m => m.GameId == game.Id && m.UserId == userId && m.CurrentScore == 8)), Times.Once);
        }

        [Fact]
        public async Task ShouldGetStatusOfExistingGame()
        {
            // Arrage
            var manager = new GameRepository();
            var game = manager.Create("my game", new[] { Guid.NewGuid().ToString() });
            var searchUsersQuery = SetupHelpers.SetupUserRepository(game.PlayerUserIds);

            var query = new GetGameQuery(game.Id);
            var handler = new GetGameQueryHandler(manager, searchUsersQuery);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(game.Id, result.Id);
            Assert.Equal(game.Title, result.Title);
            Assert.All(game.PlayerUserIds, p => result.UserScores.Any( p2 => p2.User.UserId == p));
            Assert.Equal(game.Scores, game.Scores);
        }

        private bool AreEqual<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var firstNotSecond = list1.Except(list2).ToList();
            var secondNotFirst = list2.Except(list1).ToList();

            return !firstNotSecond.Any() && !secondNotFirst.Any();
        }
    }
}
