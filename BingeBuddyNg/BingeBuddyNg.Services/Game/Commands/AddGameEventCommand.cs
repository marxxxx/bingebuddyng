using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Shared;
using MediatR;

namespace BingeBuddyNg.Core.Game.Commands
{
    public class AddGameEventCommand : IRequest
    {
        public Guid GameId { get; }
        public string UserId { get; }
        public int Count { get; }

        public AddGameEventCommand(Guid gameId, string myUserId, int count)
        {
            this.GameId = gameId;
            this.UserId = myUserId;
            this.Count = count;
        }
    }

    public class AddGameEventCommandHandler : IRequestHandler<AddGameEventCommand>
    {
        private INotificationService notificationService;
        private GameRepository manager;

        public AddGameEventCommandHandler(INotificationService notificationService, GameRepository manager)
        {
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public async Task<Unit> Handle(AddGameEventCommand command, CancellationToken cancellationToken)
        {
            var currentScore = this.manager.Get(command.GameId).IncrementScore(command.UserId, command.Count);

            var game = this.manager.Get(command.GameId);

            await this.notificationService.SendSignalRMessageAsync(
                game.PlayerUserIds.Select(u => u.ToString()).ToList().AsReadOnly(),
                Constants.SignalR.NotificationHubName,
                HubMethodNames.GameUpdateReceived,
                new GameUpdateReceivedMessage(command.GameId, command.UserId, currentScore));

            return Unit.Value;
        }
    }
}
