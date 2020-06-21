using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Infrastructure;
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
        private GameRepository gameRepository;

        public AddGameEventCommandHandler(INotificationService notificationService, GameRepository gameRepository)
        {
            this.notificationService = notificationService;
            this.gameRepository = gameRepository;
        }

        public async Task<Unit> Handle(AddGameEventCommand command, CancellationToken cancellationToken)
        {
            var game = this.gameRepository.Get(command.GameId);

            var currentScore = game.IncrementScore(command.UserId, command.Count);

            await this.notificationService.SendSignalRMessageAsync(
                game.PlayerUserIds.Select(u => u.ToString()).ToList().AsReadOnly(),
                Constants.SignalR.NotificationHubName,
                HubMethodNames.GameUpdateReceived,
                new GameUpdateReceivedMessage(command.GameId, command.UserId, currentScore));

            return Unit.Value;
        }
    }
}
