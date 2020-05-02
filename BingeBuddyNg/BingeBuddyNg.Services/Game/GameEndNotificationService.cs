using BingeBuddyNg.Services.Infrastructure;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Game
{
    public class GameEndNotificationService : IHostedService
    {
        private readonly IGameManager gameManager;
        private readonly INotificationService notificationService;

        public GameEndNotificationService(IGameManager gameManager, INotificationService notificationService)
        {
            this.gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.gameManager.GameEnded += OnGameEnded;
            return Task.CompletedTask;
        }

        private void OnGameEnded(object sender, GameEndedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.gameManager.GameEnded -= OnGameEnded;
            return Task.CompletedTask;
        }
    }
}
