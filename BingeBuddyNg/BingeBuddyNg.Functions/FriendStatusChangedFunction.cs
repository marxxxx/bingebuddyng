using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Activity.Commands;
using BingeBuddyNg.Core.Activity.Persistence;
using BingeBuddyNg.Core.Activity.Queries;
using BingeBuddyNg.Core.User.Messages;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Functions
{
    public class FriendStatusChangedFunction
    {
        private readonly GetUserActivitiesQuery getUserActivitiesQuery;
        private readonly IActivityRepository activityRepository;

        public FriendStatusChangedFunction(
            GetUserActivitiesQuery getUserActivitiesQuery, 
            IActivityRepository activityRepository)
        {
            this.getUserActivitiesQuery = getUserActivitiesQuery;
            this.activityRepository = activityRepository;
        }

        [FunctionName(nameof(FriendStatusChangedFunction))]
        public async Task Run([QueueTrigger(QueueNames.FriendStatusChanged, Connection = "AzureWebJobsStorage")] FriendStatusChangedMessage message, ILogger log)
        {
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

            log.LogInformation($"Successfully updated personalized feeds as a result of a friendship status change: {message}");
        }

        private async Task ImportActivities(string userId, IEnumerable<ActivityEntity> activities)
        {
            foreach (var a in activities)
            {
                await this.activityRepository.AddToPersonalizedFeedAsync(userId, a);
            }
        }

        private async Task RemoveActivities(string userId, IEnumerable<string> activityIds)
        {
            foreach (var id in activityIds)
            {
                await this.activityRepository.DeleteFromPersonalizedFeedAsync(userId, id);
            }
        }
    }
}
