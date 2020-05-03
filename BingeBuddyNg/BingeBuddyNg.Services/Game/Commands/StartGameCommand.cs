using BingeBuddyNg.Services.Game;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services
{
    public class StartGameCommand : IRequest<StartGameResultDTO>
    {
        public string UserId { get; }
        public string Title { get; }
        public string[] FriendUserIds { get; }

        public StartGameCommand(string myUserId, string gameTitle, string[] friendUserIds)
        {
            this.UserId = myUserId;
            this.Title = gameTitle;
            this.FriendUserIds = friendUserIds;
        }
    }
   
    public class StartGameCommandHandler : IRequestHandler<StartGameCommand, StartGameResultDTO>
    {

        private readonly INotificationService notificationService;
        private readonly IGameManager manager;
        private IUserRepository userRepository;

        public StartGameCommandHandler(INotificationService notificationService, IGameManager manager, IUserRepository userRepository)
        {
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<StartGameResultDTO> Handle(StartGameCommand command, CancellationToken cancellationToken)
        {
            var gameId = Guid.NewGuid();

            this.manager.StartGame(new Game.Game(gameId, command.Title, command.FriendUserIds));

            var friendIds = command.FriendUserIds.Select(f => f.ToString()).ToList();
            var allParticipents = new List<string>(friendIds);
            allParticipents.Add(command.UserId.ToString());

            var users = await this.userRepository.GetUsersAsync(allParticipents);
            var pushInfosOfInvitedFriends = users.Where(u => u.PushInfo != null && u.Id != command.UserId.ToString()).Select(u => u.PushInfo).ToList();

            var message = new GameStartedMessage(gameId, command.Title, command.FriendUserIds);

            await this.notificationService.SendSignalRMessageAsync(
                friendIds,
                Constants.SignalR.NotificationHubName,
                HubMethodNames.GameStarted,
                message);

            var inviter = users.FirstOrDefault(u => u.Id == command.UserId.ToString());

            string url = $"{Constants.Urls.ApplicationUrl}/game/play/{gameId}";

            var pushMessage = new NotificationMessage("Spiel gestartet", $"Du wurdest von {inviter.Name} eingeladen ein Spiel zu spielen!", url);
            this.notificationService.SendWebPushMessage(
                pushInfosOfInvitedFriends,
                pushMessage);

            return new StartGameResultDTO(gameId);
        }
    }
}
