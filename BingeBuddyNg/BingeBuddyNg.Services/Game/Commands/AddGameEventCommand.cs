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
    public class AddGameEventCommand
    {
        public Guid GameId { get; }
        public Guid UserId { get; }
        public int Count { get; }

        public AddGameEventCommand(Guid gameId, Guid myUserId, int count)
        {
            this.GameId = gameId;
            this.UserId = myUserId;
            this.Count = count;
        }
    }

    public class AddGameEventCommandHandler
    {
        public static readonly string UserScoreUpdatedMethodName = "UserScoreUpdated";

        private INotificationService notificationService;
        private IGameManager manager;

        public AddGameEventCommandHandler(INotificationService notificationService, IGameManager manager)
        {
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public async Task<Unit> Handle(AddGameEventCommand command, CancellationToken cancellationToken)
        {
            var currentScore = this.manager.AddUserScore(command.GameId, command.UserId, command.Count);

            var game = this.manager.GetGame(command.GameId);

            await this.notificationService.SendSignalRMessageAsync(
                game.PlayerUserIds.Select(u => u.ToString()).ToList().AsReadOnly(),
                Constants.SignalR.NotificationHubName,
                UserScoreUpdatedMethodName,
                new UserScoreUpdatedMessage(command.GameId, command.UserId, currentScore));

            return Unit.Value;
        }
    }
}
