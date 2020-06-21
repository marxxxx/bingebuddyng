using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;
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
            var entity = await storageAccessService.GetTableEntityAsync<UserStatsTableEntity>(TableNames.UserStats, StaticPartitionKeys.UserStats, userId);
            if (entity.Score == null)
            {
                entity.Score = 0;
            }
            entity.Score += additionalScore;

            await storageAccessService.ReplaceAsync(TableNames.UserStats, entity);
        }
    }
}
