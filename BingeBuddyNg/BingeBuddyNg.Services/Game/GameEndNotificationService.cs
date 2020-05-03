using BingeBuddyNg.Services.Infrastructure;
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

        public GameEndNotificationService(IGameManager gameManager, INotificationService notificationService, IUserRepository userRepository)
        {
            this.gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
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
            var pushInfos = users.Where(u => u.PushInfo != null).Select(u => u.PushInfo);
            var winnerUser = users.FirstOrDefault(u => u.Id == e.WinnerUserId);

            string url = $"{Constants.Urls.ApplicationUrl}/game/end/{e.Game.Id}";

            this.notificationService.SendWebPushMessage(pushInfos,
                new NotificationMessage($"{winnerUser.Name} hat gewonnen!", "Spiel beended", url));            
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.gameManager.GameEnded -= OnGameEnded;
            return Task.CompletedTask;
        }
    }
}
