using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Activity.Domain;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.User.Queries;
using BingeBuddyNg.Shared;
using Microsoft.Extensions.Hosting;

namespace BingeBuddyNg.Services.Game
{
    public class GameEndNotificationService : IHostedService
    {
        private readonly IGameManager gameManager;
        private readonly INotificationService notificationService;
        private readonly ISearchUsersQuery getUsersQuery;
        private readonly ITranslationService translationService;
        private readonly IActivityRepository activityRepository;

        public GameEndNotificationService(
            IGameManager gameManager, 
            INotificationService notificationService,
            ISearchUsersQuery getUsersQuery,
            ITranslationService translationService,
            IActivityRepository activityRepository)
        {
            this.gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.getUsersQuery = getUsersQuery ?? throw new ArgumentNullException(nameof(getUsersQuery));
            this.translationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.gameManager.GameEnded += OnGameEnded;
            return Task.CompletedTask;
        }

        private async void OnGameEnded(object sender, GameEndedEventArgs e)
        {
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

            foreach(var lang in messagesPerLanguage)
            {
                var gameOverTitle = await translationService.GetTranslationAsync(lang.Key, "Game.GameOver");
                var gameOverMessage = winnerUser != null ? await translationService.GetTranslationAsync(lang.Key, "Game.WinnerMessage", winnerUser.Name) :
                    await translationService.GetTranslationAsync(lang.Key, "Game.NoWinnerMessage");

                var languagePushInfos = lang.Select(l => l.PushInfo).ToList();
                this.notificationService.SendWebPushMessage(languagePushInfos,
                    new WebPushNotificationMessage(gameOverMessage, gameOverTitle, url));
            }

            var activity = Activity.Activity.CreateGameActivity(e.Game.ToEntity(users.Select(u=>u.ToUserInfo())), winnerUser?.ToUserInfo());
            
            var savedActivity = await this.activityRepository.AddActivityAsync(activity.ToEntity());

            await activityRepository.AddToActivityAddedTopicAsync(savedActivity.Id);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.gameManager.GameEnded -= OnGameEnded;
            return Task.CompletedTask;
        }
    }
}
