using System;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity.Messages;
using BingeBuddyNg.Functions.Services;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BingeBuddyNg.Functions
{
    public class ActivityAddedFunction
    {
        private readonly ActivityAddedService activityAddedService;

        public ActivityAddedFunction(ActivityAddedService activityAddedService)
        {
            this.activityAddedService = activityAddedService ?? throw new ArgumentNullException(nameof(activityAddedService));
        }

        [FunctionName(nameof(ActivityAddedFunction))]
        public async Task Run(
            [EventGridTrigger] EventGridEvent gridEvent,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var activityAddedMessage = JsonConvert.DeserializeObject<ActivityAddedMessage>(gridEvent.Data.ToString());

            await this.activityAddedService.RunAsync(activityAddedMessage, starter);
        }
    }
}