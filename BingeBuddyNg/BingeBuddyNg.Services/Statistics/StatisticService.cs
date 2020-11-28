using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Infrastructure;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.Statistics
{
    public sealed class StatisticService
    {
        private readonly IStorageAccessService storageAccessService;

        public StatisticService(IStorageAccessService storageAccessService)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public async Task<List<UserStatistics>> GetStatisticsAsync(IEnumerable<string> userId)
        {
            var tasks = userId.Select(u => GetStatisticsAsync(u));
            var result = await Task.WhenAll(tasks);
            return result.ToList();
        }

        public async Task<UserStatistics> GetStatisticsAsync(string userId)
        {
            var result = await storageAccessService.GetTableEntityAsync<UserStatsTableEntity>(TableNames.UserStats, StaticPartitionKeys.UserStats, userId);
            if (result != null)
            {
                return new UserStatistics(userId, result.CurrentAlcoholization, result.CurrentNightDrinks, result.Score, result.TotalDrinksLastMonth);
            }
            else
            {
                return new UserStatistics(userId);
            }
        }
    }
}
