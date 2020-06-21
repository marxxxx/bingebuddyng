using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity.Persistence;
using BingeBuddyNg.Services.Infrastructure;
using Microsoft.WindowsAzure.Storage.Table;
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
            var userFeedTable = this.storageAccessService.GetTableReference(TableNames.ActivityUserFeed);

            foreach (var userId in distributionUserIds)
            {
                var entity = new ActivityTableEntity(userId, activity.Id, activity);

                TableOperation operation = TableOperation.InsertOrReplace(entity);
                await userFeedTable.ExecuteAsync(operation);
            }
        }
    }
}
