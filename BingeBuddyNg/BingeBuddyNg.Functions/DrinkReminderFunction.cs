using System;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Functions.Services;
using BingeBuddyNg.Functions.Services.Notifications;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace BingeBuddyNg.Functions
{
    public class DrinkReminderFunction
    {
        private readonly IMonitoringRepository monitoringRepository;
        private readonly PushNotificationService notificationService;

        public DrinkReminderFunction(IMonitoringRepository monitoringRepository, PushNotificationService notificationService)
        {
            this.monitoringRepository = monitoringRepository;
            this.notificationService = notificationService;
        }

        [FunctionName(nameof(DrinkReminderFunction))]
        public async Task RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            log.LogInformation($"Running drink reminder function ...");

            var userId = context.GetInput<string>();

            log.LogInformation($"Got user {userId} ");

            await this.monitoringRepository.DeleteAsync(userId);

            log.LogInformation($"Waiting for next drink to occur.");
            await context.CreateTimer(DateTime.UtcNow.AddHours(1), CancellationToken.None);

            var notification = new DrinkReminderNotification(userId);
            await notificationService.NotifyAsync(new[] { notification });

            log.LogInformation($"Terminating monitoring instance for user {userId}");
        }
    }
}