using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Statistics;
using BingeBuddyNg.Core.Statistics.DTO;
using BingeBuddyNg.Core.Infrastructure;
using Microsoft.WindowsAzure.Storage.Table;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Services.Statistics.Querys
{
    public class GetStatisticHistoryForUserQuery
    {
        private readonly IStorageAccessService storageAccessService;

        public GetStatisticHistoryForUserQuery(IStorageAccessService storageAccessService)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public async Task<IEnumerable<UserStatisticHistoryDTO>> ExecuteAsync(string userId)
        {
            string whereClause =
                TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, DateTime.UtcNow.Subtract(TimeSpan.FromDays(1))
                .ToString("yyyyMMddHHmm")));
            var queryResult = await storageAccessService.QueryTableAsync<UserStatisticHistoryTableEntity>(TableNames.UserStatsHistory, whereClause);

            var result = queryResult
                .Select(r => new UserStatisticHistoryDTO(r.CalculationTimestamp, r.CurrentAlcLevel))
                .ToList();

            return result;
        }
    }
}
