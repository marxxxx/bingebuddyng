using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace BingeBuddyNg.Functions
{
    public class DrinkReminderFunction
    {
        public IUserRepository UserRepository { get; }
        public INotificationService NotificationService { get; }
        public ITranslationService TranslationService { get; }

        public DrinkReminderFunction(IUserRepository userRepository, INotificationService notificationService, ITranslationService translationService)
        {
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.NotificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.TranslationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
        }

        [FunctionName(nameof(DrinkReminderFunction))]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            log.LogInformation($"Running drink reminder function ...");
            
            
            User user = context.GetInput<User>();

            log.LogInformation($"Got user {user} ");

            if (user.PushInfo == null)
            {
                await UserRepository.UpdateMonitoringInstanceAsync(user.Id, null);
            }
            else
            {
                using (var cts = new CancellationTokenSource())
                {
                    log.LogInformation($"Waiting for next drink to occur.");
                    await context.CreateTimer(DateTime.UtcNow.AddHours(1), cts.Token);

                    var subject = await TranslationService.GetTranslationAsync(user.Language, "DrinkReminder");
                    var messageContent = await TranslationService.GetTranslationAsync(user.Language, "DrinkReminderMessage");

                    var reminderMessage = new NotificationMessage(subject, messageContent);

                    NotificationService.SendWebPushMessage(new[] { user.PushInfo }, reminderMessage);
                }

                await UserRepository.UpdateMonitoringInstanceAsync(user.Id, null);
                log.LogInformation($"Terminating monitoring instance for usr {user}");
            }
        }
    }
}