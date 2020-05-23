﻿using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Shared;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Game
{
    public class GameEndNotificationService : IHostedService
    {
        private readonly IGameManager gameManager;
        private readonly INotificationService notificationService;
        private readonly IUserRepository userRepository;
        private readonly ITranslationService translationService;

        public GameEndNotificationService(
            IGameManager gameManager, 
            INotificationService notificationService, 
            IUserRepository userRepository,
            ITranslationService translationService)
        {
            this.gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.translationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
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

            var users = await this.userRepository.GetUsersAsync(e.Game.PlayerUserIds);
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
                    new NotificationMessage(gameOverMessage, gameOverTitle, url));
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.gameManager.GameEnded -= OnGameEnded;
            return Task.CompletedTask;
        }
    }
}
