using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Services.Game.DTO;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User.Queries;
using BingeBuddyNg.Shared;
using MediatR;

namespace BingeBuddyNg.Core.Game.Commands
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
        private readonly GameRepository gameRepository;
        private readonly SearchUsersQuery getUsersQuery;
        private readonly INotificationService notificationService;
        private readonly ITranslationService translationService;
        private readonly IActivityRepository activityRepository;

        public StartGameCommandHandler(
            INotificationService notificationService,
            GameRepository gameRepository,
            SearchUsersQuery getUsersQuery,
            ITranslationService translationService,
            IActivityRepository activityRepository)
        {
            this.notificationService = notificationService;
            this.gameRepository = gameRepository;
            this.getUsersQuery = getUsersQuery;
            this.translationService = translationService;
            this.activityRepository = activityRepository;
        }

        public async Task<StartGameResultDTO> Handle(StartGameCommand command, CancellationToken cancellationToken)
        {
            var totalPlayers = new List<string>(command.PlayerUserIds);
            totalPlayers.Add(command.UserId);

            var game = this.gameRepository.Create(command.Title, totalPlayers);
            game.GameEnded += this.OnGameEnded;

            var friendIds = command.PlayerUserIds.Select(f => f.ToString()).ToList();
            var allParticipents = new List<string>(friendIds);
            allParticipents.Add(command.UserId.ToString());

            var users = await this.getUsersQuery.ExecuteAsync(allParticipents);
            var pushInfosOfInvitedFriends = users
                .Where(u => u.PushInfo != null && u.Id != command.UserId.ToString())
                .Select(u => new { u.Language, u.PushInfo })
                .ToList();

            var message = new GameStartedMessage(game.Id, command.Title, command.PlayerUserIds);

            await this.notificationService.SendSignalRMessageAsync(
                friendIds,
                Constants.SignalR.NotificationHubName,
                HubMethodNames.GameStarted,
                message);

            var inviter = users.FirstOrDefault(u => u.Id == command.UserId.ToString());

            string url = $"{Constants.Urls.ApplicationUrl}/game/play/{game.Id}";

            var groupedByLanguage = pushInfosOfInvitedFriends.GroupBy(p => p.Language);

            foreach(var lang in groupedByLanguage)
            {
                var translatedMessage = await this.translationService.GetTranslationAsync(lang.Key, "Game.InvitationMessage", inviter.Name);

                this.notificationService.SendWebPushMessage(
                    lang.Select(l=>l.PushInfo),
                    new WebPushNotificationMessage(command.Title, translatedMessage, url));
            }

            game.Start();

            return new StartGameResultDTO(game.Id);
        }

        private async void OnGameEnded(object sender, GameEndedEventArgs e)
        {
            e.Game.GameEnded -= this.OnGameEnded;

            await this.notificationService.SendSignalRMessageAsync(
                e.Game.PlayerUserIds,
                Constants.SignalR.NotificationHubName,
                HubMethodNames.GameEnded,
                new GameEndedMessage(e.Game.Id, e.WinnerUserId));

            var users = await this.getUsersQuery.ExecuteAsync(e.Game.PlayerUserIds);
            var pushInfos = users.Where(u => u.PushInfo != null).Select(u => new { u.Language, u.PushInfo });
            var winnerUser = users.FirstOrDefault(u => u.Id == e.WinnerUserId);

            string url = $"{Constants.Urls.ApplicationUrl}/game/end/{e.Game.Id}";

            var messagesPerLanguage = pushInfos.GroupBy(g => g.Language);

            foreach (var lang in messagesPerLanguage)
            {
                var gameOverTitle = await this.translationService.GetTranslationAsync(lang.Key, "Game.GameOver");
                var gameOverMessage = winnerUser != null ? await translationService.GetTranslationAsync(lang.Key, "Game.WinnerMessage", winnerUser.Name) :
                    await translationService.GetTranslationAsync(lang.Key, "Game.NoWinnerMessage");

                var languagePushInfos = lang.Select(l => l.PushInfo).ToList();
                this.notificationService.SendWebPushMessage(languagePushInfos,
                    new WebPushNotificationMessage(gameOverMessage, gameOverTitle, url));
            }

            var activity = Activity.Domain.Activity.CreateGameActivity(e.Game.ToEntity(users.Select(u => u.ToUserInfo())), winnerUser?.ToUserInfo());

            var savedActivity = await this.activityRepository.AddActivityAsync(activity.ToEntity());

            await activityRepository.AddToActivityAddedTopicAsync(savedActivity.Id);
        }
    }
}
