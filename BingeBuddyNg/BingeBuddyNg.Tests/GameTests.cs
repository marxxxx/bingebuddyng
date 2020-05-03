using BingeBuddyNg.Services;
using BingeBuddyNg.Services.Game;
using BingeBuddyNg.Services.Game.Queries;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            string[] friendUserIds = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            string[] friendUserIdsAsString = friendUserIds.Select(f => f.ToString()).ToArray();
            string gameTitle = "My Game";
            var notificationServiceMock = new Mock<INotificationService>();
            var manager = new GameManager();

            var userRepository = new Mock<IUserRepository>();
            userRepository
                .Setup(u => u.GetUsersAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync((IEnumerable<string> _userIds) =>
                    _userIds.Select(u => new User() { Id = u, PushInfo = new PushInfo("url", "auth", "p256dh") }).ToList()
                );
            
            var command = new StartGameCommand(myUserId, gameTitle, friendUserIds);
            var handler = new StartGameCommandHandler(notificationServiceMock.Object, manager, userRepository.Object);

            // Act
            StartGameResultDTO result = await handler.Handle(command, CancellationToken.None);

            // Assert
            var game = manager.GetGame(result.GameId);
            Assert.Equal(result.GameId, game.Id);

            Assert.NotNull(result);
            Assert.NotEqual(default(Guid), result.GameId);
            notificationServiceMock.Verify(s =>
                s.SendSignalRMessageAsync(
                    It.Is<IReadOnlyList<string>>(u => AreEqual(u, friendUserIdsAsString)), 
                    Shared.Constants.SignalR.NotificationHubName, 
                    HubMethodNames.GameStarted,
                    It.Is<GameStartedMessage>( m => m.GameId == result.GameId && m.Title ==  gameTitle && AreEqual<string>(friendUserIds, m.UserIds))), Times.Once);

            notificationServiceMock.Verify(s =>
               s.SendWebPushMessage(It.IsAny<IEnumerable<PushInfo>>(), It.Is<NotificationMessage>(m => m.data.url.Contains(game.Id.ToString()))), Times.Once);
        }

        [Fact]
        public async Task ShouldSendMessageWithIncrementedScoreWhenGameEventWasSent()
        {
            // Arrange
            Guid gameId = Guid.NewGuid();
            string userId = Guid.NewGuid().ToString();
            string[] friendUserIds = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            string[] friendUserIdsAsString = friendUserIds.Select(f => f.ToString()).ToArray();
            var notificationServiceMock = new Mock<INotificationService>();
            var manager = new GameManager();
            manager.StartGame(new Game(gameId, "my game", friendUserIds));

            var command = new AddGameEventCommand(gameId, userId, 5);
            var handler = new AddGameEventCommandHandler(notificationServiceMock.Object, manager);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            notificationServiceMock.Verify(s =>
                s.SendSignalRMessageAsync(
                    It.Is<IReadOnlyList<string>>(u => AreEqual(u, friendUserIdsAsString)),
                    Shared.Constants.SignalR.NotificationHubName,
                    HubMethodNames.GameUpdateReceived,
                    It.Is<GameUpdateReceivedMessage>(m => m.GameId == gameId && m.UserId == userId && m.CurrentScore == 5)), Times.Once);

            var secondCommand = new AddGameEventCommand(gameId, userId, 3);
            await handler.Handle(secondCommand, CancellationToken.None);

            notificationServiceMock.Verify(s =>
                s.SendSignalRMessageAsync(
                    It.Is<IReadOnlyList<string>>(u => AreEqual(u, friendUserIdsAsString)),
                    Shared.Constants.SignalR.NotificationHubName,
                    HubMethodNames.GameUpdateReceived,
                    It.Is<GameUpdateReceivedMessage>(m => m.GameId == gameId && m.UserId == userId && m.CurrentScore == 8)), Times.Once);
        }

        [Fact]
        public async Task ShouldGetStatusOfExistingGame()
        {
            // Arrage
            var gameId = Guid.NewGuid();
            var manager = new GameManager();
            var game = new Game(gameId, "my game", new[] { Guid.NewGuid().ToString() });
            manager.StartGame(game);
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock
                .Setup(r => r.GetUsersAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync((IEnumerable<string> _userIds) => _userIds.Select(_u => new User() { Id = _u, Name = _u })
                .ToList());

            var query = new GetGameQuery(gameId);
            var handler = new GetGameQueryHandler(manager, userRepositoryMock.Object);

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
