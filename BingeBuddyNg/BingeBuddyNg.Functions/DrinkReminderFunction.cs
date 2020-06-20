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
        private readonly IUserRepository userRepository;
        private readonly PushNotificationService notificationService;

        public DrinkReminderFunction(IUserRepository userRepository, PushNotificationService notificationService)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        [FunctionName(nameof(DrinkReminderFunction))]
        public async Task RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            log.LogInformation($"Running drink reminder function ...");

            User user = context.GetInput<User>();

            log.LogInformation($"Got user {user} ");

            if (user.PushInfo == null)
            {
                user.UpdateMonitoringInstance(null);
                await userRepository.UpdateUserAsync(user.ToEntity());
                return;
            }

            log.LogInformation($"Waiting for next drink to occur.");
            await context.CreateTimer(DateTime.UtcNow.AddHours(1), CancellationToken.None);

            var notification = new DrinkReminderNotification(user.Id);
            await notificationService.NotifyAsync(new[] { notification });

            user.UpdateMonitoringInstance(null);
            await userRepository.UpdateUserAsync(user.ToEntity());
            log.LogInformation($"Terminating monitoring instance for user {user}");
        }
    }
}