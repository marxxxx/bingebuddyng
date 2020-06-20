using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Game;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User.Queries;
using BingeBuddyNg.Shared;
using MediatR;

namespace BingeBuddyNg.Services
{
    public class StartGameCommand : IRequest<StartGameResultDTO>
    {
        public string UserId { get; }
        public string Title { get; }
        public string[] PlayerUserIds { get; }

        public StartGameCommand(string myUserId, string gameTitle, string[] friendUserIds)
        {
            this.UserId = myUserId;
            this.Title = gameTitle;
            this.PlayerUserIds = friendUserIds;
        }
    }

    public class StartGameCommandHandler : IRequestHandler<StartGameCommand, StartGameResultDTO>
    {
        private readonly INotificationService notificationService;
        private readonly IGameManager manager;
        private readonly ISearchUsersQuery getUsersQuery;
        private readonly ITranslationService translationServie;

        public StartGameCommandHandler(
            INotificationService notificationService,
            IGameManager manager,
            ISearchUsersQuery getUsersQuery,
            ITranslationService translationService)
        {
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
            this.getUsersQuery = getUsersQuery ?? throw new ArgumentNullException(nameof(getUsersQuery));
            this.translationServie = translationService ?? throw new ArgumentNullException(nameof(translationService));
        }

        public async Task<StartGameResultDTO> Handle(StartGameCommand command, CancellationToken cancellationToken)
        {
            var gameId = Guid.NewGuid();

            var totalPlayers = new List<string>(command.PlayerUserIds);
            totalPlayers.Add(command.UserId);

            this.manager.StartGame(new Game.Game(gameId, command.Title, totalPlayers));

            var friendIds = command.PlayerUserIds.Select(f => f.ToString()).ToList();
            var allParticipents = new List<string>(friendIds);
            allParticipents.Add(command.UserId.ToString());

            var users = await this.getUsersQuery.ExecuteAsync(allParticipents);
            var pushInfosOfInvitedFriends = users
                .Where(u => u.PushInfo != null && u.Id != command.UserId.ToString())
                .Select(u => new { u.Language, u.PushInfo })
                .ToList();

            var message = new GameStartedMessage(gameId, command.Title, command.PlayerUserIds);

            await this.notificationService.SendSignalRMessageAsync(
                friendIds,
                Constants.SignalR.NotificationHubName,
                HubMethodNames.GameStarted,
                message);

            var inviter = users.FirstOrDefault(u => u.Id == command.UserId.ToString());

            string url = $"{Constants.Urls.ApplicationUrl}/game/play/{gameId}";

            var groupedByLanguage = pushInfosOfInvitedFriends.GroupBy(p => p.Language);

            foreach(var lang in groupedByLanguage)
            {
                var translatedMessage = await this.translationServie.GetTranslationAsync(lang.Key, "Game.InvitationMessage", inviter.Name);

                this.notificationService.SendWebPushMessage(
                    lang.Select(l=>l.PushInfo),
                    new WebPushNotificationMessage(command.Title, translatedMessage, url));
            }

            return new StartGameResultDTO(gameId);
        }
    }
}
