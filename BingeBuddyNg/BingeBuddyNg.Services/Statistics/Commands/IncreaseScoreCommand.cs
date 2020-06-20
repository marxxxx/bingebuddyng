using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;
using Microsoft.WindowsAzure.Storage.Table;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.Statistics.Commands
{
    public class IncreaseScoreCommand
    {
        private readonly IStorageAccessService storageAccessService;

        public IncreaseScoreCommand(IStorageAccessService storageAccessService)
        {
            this.storageAccessService = storageAccessService;
        }

        public async Task ExecuteAsync(string userId, int additionalScore)
        {
            var table = storageAccessService.GetTableReference(TableNames.UserStats);

            var result = await storageAccessService.GetTableEntityAsync<UserStatsTableEntity>(TableNames.UserStats, StaticPartitionKeys.UserStats, userId);
            if (result.Score == null)
            {
                result.Score = 0;
            }
            result.Score += additionalScore;

            TableOperation saveOperation = TableOperation.Replace(result);

            await table.ExecuteAsync(saveOperation);
        }
    }
}
