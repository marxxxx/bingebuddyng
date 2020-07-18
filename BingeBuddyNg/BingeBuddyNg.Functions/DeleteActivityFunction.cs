using System;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Activity.Commands;
using BingeBuddyNg.Core.Activity.Messages;
using BingeBuddyNg.Core.User.Queries;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Functions
{
    public class DeleteActivityFunction
    {
        private readonly IActivityRepository activityRepository;
        private readonly GetAllUserIdsQuery getAllUserIdsQuery;

        public DeleteActivityFunction(IActivityRepository activityRepository, GetAllUserIdsQuery getAllUserIdsQuery)
        {
            this.activityRepository = activityRepository;
            this.getAllUserIdsQuery = getAllUserIdsQuery;
        }

        [FunctionName(nameof(DeleteActivityFunction))]
        public async Task Run([QueueTrigger(QueueNames.DeleteActivity, Connection = "AzureWebJobsStorage")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"Deleting activity from personalized feeds: {myQueueItem}");

            var message = JsonConvert.DeserializeObject<DeleteActivityMessage>(myQueueItem);

            var allUserIds = await this.getAllUserIdsQuery.ExecuteAsync();

            foreach (var userId in allUserIds)
            {
                try
                {
                    await this.activityRepository.DeleteFromPersonalizedFeedAsync(userId, message.ActivityId);
                }
                catch (Exception ex)
                {
                    log.LogError(ex, $"Failed to delete activity [{message.ActivityId}] from personalized feed of user [{userId}]");
                }
            }

            log.LogInformation($"Successfully deleted activity [{message.ActivityId}] from personalized feeds.");
        }
    }
}
