using System;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Activity.Messages;
using BingeBuddyNg.Core.User;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Functions
{
    public class DeleteActivityFunction
    {
        private readonly IActivityRepository activityRepository;
        private readonly IUserRepository userRepository;

        public DeleteActivityFunction(IActivityRepository activityRepository, IUserRepository userRepository)
        {
            this.activityRepository = activityRepository;
            this.userRepository = userRepository;
        }

        [FunctionName(nameof(DeleteActivityFunction))]
        public async Task Run([QueueTrigger(QueueNames.DeleteActivity, Connection = "AzureWebJobsStorage")] DeleteActivityMessage message, ILogger log)
        {
            log.LogInformation($"Deleting activity from personalized feeds: {message}");

            var allUserIds = await this.userRepository.GetAllUserIdsAsync();

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
