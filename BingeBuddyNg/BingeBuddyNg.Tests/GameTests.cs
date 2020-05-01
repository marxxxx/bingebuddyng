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
            Guid myUserId = Guid.NewGuid();
            Guid[] friendUserIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
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

            StartGameResult result = await handler.Handle(command, CancellationToken.None);

            var game = manager.GetGame(result.GameId);
            Assert.Equal(result.GameId, game.Id);

            Assert.NotNull(result);
            Assert.NotEqual(default(Guid), result.GameId);
            notificationServiceMock.Verify(s =>
                s.SendSignalRMessageAsync(
                    It.Is<IReadOnlyList<string>>(u => AreEqual(u, friendUserIdsAsString)), 
                    Shared.Constants.SignalR.NotificationHubName, 
                    StartGameCommandHandler.GameStartedMethodName,
                    It.Is<GameStartedMessage>( m => m.GameId == result.GameId && m.Title ==  gameTitle && AreEqual(friendUserIds, m.UserIds))), Times.Once);

            notificationServiceMock.Verify(s =>
               s.SendWebPushMessage(It.IsAny<IEnumerable<PushInfo>>(), It.Is<NotificationMessage>(m => m.data.url.Contains(game.Id.ToString()))), Times.Once);
        }

        [Fact]
        public async Task ShouldSendMessageWithIncrementedScoreWhenGameEventWasSent()
        {
            Guid gameId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            Guid[] friendUserIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
            string[] friendUserIdsAsString = friendUserIds.Select(f => f.ToString()).ToArray();
            var notificationServiceMock = new Mock<INotificationService>();
            var manager = new GameManager();
            manager.CreateGame(new Game(gameId, "my game", friendUserIds));

            var command = new AddGameEventCommand(gameId, userId, 5);
            var handler = new AddGameEventCommandHandler(notificationServiceMock.Object, manager);

            await handler.Handle(command, CancellationToken.None);

            notificationServiceMock.Verify(s =>
                s.SendSignalRMessageAsync(
                    It.Is<IReadOnlyList<string>>(u => AreEqual(u, friendUserIdsAsString)),
                    Shared.Constants.SignalR.NotificationHubName,
                    AddGameEventCommandHandler.GameUpdateReceivedMethodName,
                    It.Is<GameUpdateReceivedMessage>(m => m.GameId == gameId && m.UserId == userId && m.CurrentScore == 5)), Times.Once);

            var secondCommand = new AddGameEventCommand(gameId, userId, 3);
            await handler.Handle(secondCommand, CancellationToken.None);

            notificationServiceMock.Verify(s =>
                s.SendSignalRMessageAsync(
                    It.Is<IReadOnlyList<string>>(u => AreEqual(u, friendUserIdsAsString)),
                    Shared.Constants.SignalR.NotificationHubName,
                    AddGameEventCommandHandler.GameUpdateReceivedMethodName,
                    It.Is<GameUpdateReceivedMessage>(m => m.GameId == gameId && m.UserId == userId && m.CurrentScore == 8)), Times.Once);
        }

        [Fact]
        public async Task ShouldGetStatusOfExistingGame()
        {
            var gameId = Guid.NewGuid();
            var manager = new GameManager();
            var game = new Game(gameId, "my game", new[] { Guid.NewGuid() });
            manager.CreateGame(game);

            var query = new GetGameStatusQuery(gameId);
            var handler = new GetGameStatusQueryHandler(manager);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal(game.Id, result.Id);
            Assert.Equal(game.Title, result.Title);
            Assert.Equal(game.PlayerUserIds, game.PlayerUserIds);
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
