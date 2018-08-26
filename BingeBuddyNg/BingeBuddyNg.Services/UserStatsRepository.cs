using BingeBuddyNg.Services.Entitys;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services
{
    public class UserStatsRepository : IUserStatsRepository
    {
        private const string TableName = "userstats";
        private const string PartitionKeyValue = "UserStats";

        public StorageAccessService StorageAccessService { get; }

        public UserStatsRepository(StorageAccessService storageAccessService)
        {
            StorageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }


        public async Task<List<UserStatistics>> GetStatisticsAsync(IEnumerable<string> userId)
        {
            var tasks = userId.Select(u => GetStatisticsAsync(u));
            var result = await Task.WhenAll(tasks);
            return result.ToList();
        }

        public async Task<UserStatistics> GetStatisticsAsync(string userId)
        {
            var result = await StorageAccessService.GetTableEntityAsync<UserStatsTableEntity>(TableName, PartitionKeyValue, userId);
            if (result != null)
            {
                return new UserStatistics(userId, result.CurrentAlcoholization, result.CurrentNightDrinks);
            }
            else
            {
                return new UserStatistics(userId);
            }

        }

        public Task SaveStatisticsForUserAsync(UserStatistics userStatistics)
        {
            var table = StorageAccessService.GetTableReference(TableName);

            TableOperation saveOperation = TableOperation.InsertOrReplace(new UserStatsTableEntity(PartitionKeyValue, userStatistics.UserId, userStatistics.CurrentAlcoholization, userStatistics.CurrentNightDrinks));

            return table.ExecuteAsync(saveOperation);
        }
    }
}
