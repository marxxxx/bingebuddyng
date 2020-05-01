using BingeBuddyNg.Services;
using BingeBuddyNg.Services.Game;
using BingeBuddyNg.Services.Infrastructure;
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
        public async Task ShouldCreateGameAndSendNotificationWhenGameWasStarted()
        {
            Guid myUserId = Guid.NewGuid();
            Guid[] friendUserIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
            string[] friendUserIdsAsString = friendUserIds.Select(f => f.ToString()).ToArray();
            string gameTitle = "My Game";
            var notificationServiceMock = new Mock<INotificationService>();
            var manager = new GameManager();
            
            var command = new StartGameCommand(myUserId, gameTitle, friendUserIds);
            var handler = new StartGameCommandHandler(notificationServiceMock.Object, manager);

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
        }

        [Fact]
        public async Task ShouldSendMessageWithIncrementedScoreWhenGameEventWasSent()
        {
            Guid userId = Guid.NewGuid();
            Guid[] friendUserIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
            string[] friendUserIdsAsString = friendUserIds.Select(f => f.ToString()).ToArray();
            var notificationServiceMock = new Mock<INotificationService>();
            var manager = new GameManager();

            var startCommand = new StartGameCommand(userId, "any title", friendUserIds);
            var startHandler = new StartGameCommandHandler(notificationServiceMock.Object, manager);
            var startResult = await startHandler.Handle(startCommand, CancellationToken.None);

            var command = new AddGameEventCommand(startResult.GameId, userId, 5);
            var handler = new AddGameEventCommandHandler(notificationServiceMock.Object, manager);

            await handler.Handle(command, CancellationToken.None);

            notificationServiceMock.Verify(s =>
                s.SendSignalRMessageAsync(
                    It.Is<IReadOnlyList<string>>(u => AreEqual(u, friendUserIdsAsString)),
                    Shared.Constants.SignalR.NotificationHubName,
                    AddGameEventCommandHandler.UserScoreUpdatedMethodName,
                    It.Is<UserScoreUpdatedMessage>(m => m.GameId == startResult.GameId && m.UserId == userId && m.CurrentScore == 5)), Times.Once);

            var secondCommand = new AddGameEventCommand(startResult.GameId, userId, 3);
            await handler.Handle(secondCommand, CancellationToken.None);

            notificationServiceMock.Verify(s =>
                s.SendSignalRMessageAsync(
                    It.Is<IReadOnlyList<string>>(u => AreEqual(u, friendUserIdsAsString)),
                    Shared.Constants.SignalR.NotificationHubName,
                    AddGameEventCommandHandler.UserScoreUpdatedMethodName,
                    It.Is<UserScoreUpdatedMessage>(m => m.GameId == startResult.GameId && m.UserId == userId && m.CurrentScore == 8)), Times.Once);
        }

        private bool AreEqual<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var firstNotSecond = list1.Except(list2).ToList();
            var secondNotFirst = list2.Except(list1).ToList();

            return !firstNotSecond.Any() && !secondNotFirst.Any();
        }
    }
}
