using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;
using Microsoft.WindowsAzure.Storage.Table;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.Activity.Commands
{
    public class DeleteActivityFromPersonalizedFeedCommand
    {
        private readonly IStorageAccessService storageAccessService;

        public DeleteActivityFromPersonalizedFeedCommand(IStorageAccessService storageAccessService)
        {
            this.storageAccessService = storageAccessService;
        }

        public async Task ExecuteAsync(string userId, string id)
        {
            var userFeedTable = this.storageAccessService.GetTableReference(TableNames.ActivityUserFeed);

            TableOperation retrieveOperation = TableOperation.Retrieve<ActivityTableEntity>(userId, id);
            var result = await userFeedTable.ExecuteAsync(retrieveOperation);

            var entity = (ActivityTableEntity)result.Result;

            if (entity != null)
            {
                await userFeedTable.ExecuteAsync(TableOperation.Delete(entity));
            }
        }
    }
}
