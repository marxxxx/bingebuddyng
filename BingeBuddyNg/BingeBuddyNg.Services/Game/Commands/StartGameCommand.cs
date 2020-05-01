using BingeBuddyNg.Services.Game;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Shared;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services
{
    public class StartGameCommand : IRequest<StartGameResult>
    {
        public Guid UserId { get; }
        public string GameTitle { get; }
        public Guid[] FriendUserIds { get; }

        public StartGameCommand(Guid myUserId, string gameTitle, Guid[] friendUserIds)
        {
            this.UserId = myUserId;
            this.GameTitle = gameTitle;
            this.FriendUserIds = friendUserIds;
        }
    }

    public class StartGameResult
    {
        public StartGameResult(Guid gameId)
        {
            GameId = gameId;
        }

        public Guid GameId { get; set; }
    }

    public class StartGameCommandHandler : IRequestHandler<StartGameCommand, StartGameResult>
    {
        public static readonly string GameStartedMethodName = "GameStarted";

        private readonly INotificationService notificationService;
        private readonly IGameManager manager;

        public StartGameCommandHandler(INotificationService notificationService, IGameManager manager)
        {
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public async Task<StartGameResult> Handle(StartGameCommand command, CancellationToken cancellationToken)
        {
            var gameId = Guid.NewGuid();

            this.manager.CreateGame(new Game.Game(gameId, command.FriendUserIds));

            var friendIds = command.FriendUserIds.Select(f => f.ToString()).ToList().AsReadOnly();
            var message = new GameStartedMessage(gameId, command.GameTitle, command.FriendUserIds);

            await this.notificationService.SendSignalRMessageAsync(
                friendIds,
                Constants.SignalR.NotificationHubName,
                GameStartedMethodName,
                message);

            return new StartGameResult(gameId);
        }
    }
}
