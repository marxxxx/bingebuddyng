using BingeBuddyNg.Services.Infrastructure;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Statistics
{
    public class UserStatsHistoryRepository : IUserStatsHistoryRepository
    {
        private const string TableName = "userstatshistory";

        public IStorageAccessService StorageAccessService { get; }

        public UserStatsHistoryRepository(IStorageAccessService storageAccessService)
        {
            StorageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }


        public async Task<List<UserStatisticHistory>> GetStatisticHistoryForUsersAsync(string userId)
        {
            string whereClause =
                TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, DateTime.UtcNow.Subtract(TimeSpan.FromDays(1))
                .ToString("yyyyMMddHHmm")));
            var queryResult = await StorageAccessService.QueryTableAsync<UserStatisticHistoryTableEntity>(TableName, whereClause);

            var result = queryResult
                .Select(r => new UserStatisticHistory(r.PartitionKey, r.CalculationTimestamp, r.CurrentAlcLevel))
                .ToList();

            return result;
        }




        public Task SaveStatisticsHistoryAsync(UserStatisticHistory userStatistics)
        {
            var table = StorageAccessService.GetTableReference(TableName);

            var entity = new UserStatisticHistoryTableEntity(userStatistics.UserId, DateTime.UtcNow,
                userStatistics.CurrentAlcLevel);

            TableOperation saveOperation = TableOperation.Insert(entity);

            return table.ExecuteAsync(saveOperation);
        }
    }
}
