using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Functions
{
    public static class DrinkReminderFunction
    {
        public const string FunctionNameValue = "DrinkReminderFunction";

        public static IUserRepository UserRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IUserRepository>();
        public static INotificationService NotificationService = ServiceProviderBuilder.Instance.Value.GetRequiredService<INotificationService>();
        public static readonly ITranslationService TranslationService = ServiceProviderBuilder.Instance.Value.GetRequiredService<ITranslationService>();

        [FunctionName(FunctionNameValue)]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context, ILogger log)
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
                    await context.CreateTimer(DateTime.UtcNow.AddMinutes(30), cts.Token);

                    var subject = await TranslationService.GetTranslationAsync(user.Language, "DrinkReminder");
                    var messageContent = await TranslationService.GetTranslationAsync(user.Language, "DrinkReminderMessage");

                    var reminderMessage = new Services.Models.NotificationMessage(subject, messageContent);

                    NotificationService.SendMessage(new[] { user.PushInfo }, reminderMessage);
                }

                await UserRepository.UpdateMonitoringInstanceAsync(user.Id, null);
                log.LogInformation($"Terminating monitoring instance for usr {user}");
            }
        }
    }
}