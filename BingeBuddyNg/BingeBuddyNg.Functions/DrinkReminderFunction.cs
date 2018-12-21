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

                    var reminderMessage = new Services.Models.NotificationMessage("Trink-Erinnerung",
                        "Schon lang nichts mehr getrunken. Hast du eh nicht vergessen, einzubuchen?");

                    NotificationService.SendMessage(new[] { user.PushInfo }, reminderMessage);
                }

                await UserRepository.UpdateMonitoringInstanceAsync(user.Id, null);
                log.LogInformation($"Terminating monitoring instance for usr {user}");
            }
        }

        //[FunctionName("DrinkReminderFunction_Hello")]
        //public static string SayHello([ActivityTrigger] string name, ILogger log)
        //{
        //    log.LogInformation($"Saying hello to {name}.");
        //    return $"Hello {name}!";
        //}

        //[FunctionName("DrinkReminderFunction_HttpStart")]
        //public static async Task<HttpResponseMessage> HttpStart(
        //    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
        //    [OrchestrationClient]DurableOrchestrationClient starter,
        //    ILogger log)
        //{
        //    // Function input comes from the request content.
        //    string instanceId = await starter.StartNewAsync("DrinkReminderFunction", null);

        //    log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

        //    return starter.CreateCheckStatusResponse(req, instanceId);
        //}
    }
}