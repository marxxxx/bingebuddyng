using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity.Persistence;
using BingeBuddyNg.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.Activity.Commands
{
    public class DistributeActivityToPersonalizedFeedCommand
    {
        private readonly IStorageAccessService storageAccessService;
        private readonly ILogger<DistributeActivityToPersonalizedFeedCommand> logger;

        public DistributeActivityToPersonalizedFeedCommand(
            IStorageAccessService storageAccessService,
            ILogger<DistributeActivityToPersonalizedFeedCommand> logger)
        {
            this.storageAccessService = storageAccessService;
            this.logger = logger;
        }

        public async Task ExecuteAsync(IEnumerable<string> distributionUserIds, ActivityEntity activity)
        {
            foreach (var userId in distributionUserIds)
            {
                try
                {
                    var entity = new ActivityTableEntity(userId, activity.Id, activity);

                    await this.storageAccessService.InsertAsync(TableNames.ActivityUserFeed, entity);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Failed to distribute activity to [{userId}].");
                }
            }
        }
    }
}
