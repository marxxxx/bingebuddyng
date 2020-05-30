using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace BingeBuddyNg.Functions
{
    public class ActivityDistributionFunction
    {
        private readonly IActivityRepository activityRepository;
        private readonly IUserRepository userRepository;
        private readonly IStorageAccessService storageAccessService;

        public ActivityDistributionFunction(
            IActivityRepository activityRepository,
           IUserRepository userRepository,
           IStorageAccessService storageAccessService)
        {
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        [FunctionName(nameof(ActivityDistributionFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var activityKeys = await this.storageAccessService.GetRowKeysAsync("activity", "80-07");
            var allUserIds = await this.userRepository.GetAllUserIdsAsync();
            int count = 0;
            foreach (var id in activityKeys)
            {
                var activity = await this.activityRepository.GetActivityAsync(id);

                IEnumerable<string> userIds = null;
                if (activity.ActivityType == ActivityType.Registration)
                {
                    userIds = allUserIds;
                }
                else
                {
                    var user = await this.userRepository.FindUserAsync(activity.UserId);
                    if (user != null)
                    {
                        userIds = user.GetVisibleFriendUserIds(true);
                    }
                }

                log.LogInformation($"[{count} / {activityKeys.Count()}] Distributing activity {id} to {userIds.Count()} users.");


                await this.activityRepository.DistributeActivityAsync(userIds, activity);

                count++;
            }

            return new OkResult();
        }
    }
}
