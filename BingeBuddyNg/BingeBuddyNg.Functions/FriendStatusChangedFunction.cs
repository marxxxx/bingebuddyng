using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity.Commands;
using BingeBuddyNg.Core.Activity.Persistence;
using BingeBuddyNg.Core.Activity.Queries;
using BingeBuddyNg.Services.User.Messages;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Functions
{
    public class FriendStatusChangedFunction
    {
        private readonly GetUserActivitiesQuery getUserActivitiesQuery;
        private readonly DistributeActivityToPersonalizedFeedCommand distributeActivityToPersonalizedFeedCommand;
        private readonly DeleteActivityFromPersonalizedFeedCommand deleteActivityFromPersonalizedFeedCommand;

        public FriendStatusChangedFunction(
            GetUserActivitiesQuery getUserActivitiesQuery, 
            DistributeActivityToPersonalizedFeedCommand distributeActivityToPersonalizedFeedCommand,
            DeleteActivityFromPersonalizedFeedCommand deleteActivityFromPersonalizedFeedCommand)
        {
            this.getUserActivitiesQuery = getUserActivitiesQuery;
            this.distributeActivityToPersonalizedFeedCommand = distributeActivityToPersonalizedFeedCommand;
            this.deleteActivityFromPersonalizedFeedCommand = deleteActivityFromPersonalizedFeedCommand;
        }

        [FunctionName(nameof(FriendStatusChangedFunction))]
        public async Task Run([QueueTrigger(QueueNames.FriendStatusChanged, Connection = "AzureWebJobsStorage")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"Updating personalized feeds as a result of a friendship status change: {myQueueItem}");

            var message = JsonConvert.DeserializeObject<FriendStatusChangedMessage>(myQueueItem);

            var startTime = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30));
            var userActivities = await getUserActivitiesQuery.ExecuteAsync(message.UserId, startTime);
            var friendActivities = await getUserActivitiesQuery.ExecuteAsync(message.FriendUserId, startTime);

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

        private async Task ImportActivities(string userId, IEnumerable<ActivityEntity> activities)
        {
            foreach (var a in activities)
            {
                await this.distributeActivityToPersonalizedFeedCommand.ExecuteAsync(new[] { userId }, a);
            }
        }

        private async Task RemoveActivities(string userId, IEnumerable<string> activityIds)
        {
            foreach (var id in activityIds)
            {
                await this.deleteActivityFromPersonalizedFeedCommand.ExecuteAsync(userId, id);
            }
        }
    }
}
