using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity.Persistence;
using BingeBuddyNg.Core.Infrastructure;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.Activity.Commands
{
    public class DistributeActivityToPersonalizedFeedCommand
    {
        private readonly IStorageAccessService storageAccessService;

        public DistributeActivityToPersonalizedFeedCommand(IStorageAccessService storageAccessService)
        {
            this.storageAccessService = storageAccessService;
        }

        public async Task ExecuteAsync(IEnumerable<string> distributionUserIds, ActivityEntity activity)
        {
            foreach (var userId in distributionUserIds)
            {
                var entity = new ActivityTableEntity(userId, activity.Id, activity);

                await this.storageAccessService.InsertAsync(TableNames.ActivityUserFeed, entity);
            }
        }
    }
}
