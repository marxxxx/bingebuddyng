using System;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Activity.Messages;
using BingeBuddyNg.Services.User;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Functions
{
    public class DeleteActivityFunction
    {
        private readonly IActivityRepository activityRepository;
        private readonly IUserRepository userRepository;

        public DeleteActivityFunction(IActivityRepository activityRepository, IUserRepository userRepository)
        {
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        [FunctionName(nameof(DeleteActivityFunction))]
        public async Task Run([QueueTrigger(QueueNames.DeleteActivity, Connection = "AzureWebJobsStorage")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"Deleting activity from personalized feeds: {myQueueItem}");

            var message = JsonConvert.DeserializeObject<DeleteActivityMessage>(myQueueItem);

            var allUserIds = await this.userRepository.GetAllUserIdsAsync();

            foreach (var userId in allUserIds)
            {
                try
                {
                    await this.activityRepository.DeleteActivityFromPersonalizedFeedAsync(userId, message.ActivityId);
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
