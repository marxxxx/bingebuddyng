using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.User.Messages;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Functions
{
    public class FriendStatusChangedFunction
    {
        private readonly IUserRepository userRepository;
        private readonly IActivityRepository activityRepository;

        public FriendStatusChangedFunction(IUserRepository userRepository, IActivityRepository activityRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        [FunctionName(nameof(FriendStatusChangedFunction))]
        public async Task Run([QueueTrigger(QueueNames.FriendStatusChanged, Connection = "AzureWebJobsStorage")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"Updating personalized feeds as a result of a friendship status change: {myQueueItem}");

            var message = JsonConvert.DeserializeObject<FriendStatusChangedMessage>(myQueueItem);

            var startTime = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30));
            var userActivities = await activityRepository.GetUserActivitiesAsync(message.UserId, startTime);
            var friendActivities = await activityRepository.GetUserActivitiesAsync(message.FriendUserId, startTime);

            switch (message.Status)
            {
                case FriendStatus.Added:
                    {
                        await ImportActivities(message.FriendUserId, userActivities);
                        await ImportActivities(message.UserId, friendActivities);
                        break;
                    }
                case FriendStatus.Removed:
                    {
                        await RemoveActivities(message.FriendUserId, userActivities.Select(a => a.Id));
                        await RemoveActivities(message.UserId, friendActivities.Select(a => a.Id));
                        break;
                    }
            }

            log.LogInformation($"Successfully updated personalized feeds as a result of a friendship status change: {myQueueItem}");
        }

        private async Task ImportActivities(string userId, IEnumerable<ActivityDTO> activities)
        {
            foreach (var a in activities)
            {
                await this.activityRepository.DistributeActivityAsync(new[] { userId }, a.ToDomain());
            }
        }

        private async Task RemoveActivities(string userId, IEnumerable<string> activityIds)
        {
            foreach (var id in activityIds)
            {
                await this.activityRepository.DeleteActivityFromPersonalizedFeedAsync(userId, id);
            }
        }
    }
}
