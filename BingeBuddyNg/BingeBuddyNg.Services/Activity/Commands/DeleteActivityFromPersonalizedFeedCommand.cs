using System.Threading.Tasks;
using BingeBuddyNg.Core.Infrastructure;
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
            var entity = await this.storageAccessService.GetTableEntityAsync<ActivityTableEntity>(TableNames.ActivityUserFeed, userId, id);

            if (entity != null)
            {
                await this.storageAccessService.DeleteAsync(TableNames.ActivityUserFeed, entity);
            }
        }
    }
}
