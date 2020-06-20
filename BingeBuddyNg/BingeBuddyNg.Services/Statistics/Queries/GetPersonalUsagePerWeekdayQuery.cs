using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Statistics.DTO;
using BingeBuddyNg.Services.Infrastructure;
using Microsoft.WindowsAzure.Storage.Table;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.Statistics.Queries
{
    public class GetPersonalUsagePerWeekdayQuery
    {
        private readonly IStorageAccessService storageAccessService;

        private static readonly Dictionary<int, string> orderedWeekdays = new Dictionary<int, string>()
        {
            {1, "Mon" },
            {2, "Tue" },
            {3, "Wed" },
            {4, "Thu" },
            {5, "Fri" },
            {6, "Sat" },
            {7, "Sun" }
        };

        public GetPersonalUsagePerWeekdayQuery(IStorageAccessService storageAccessService)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public async Task<IEnumerable<PersonalUsagePerWeekdayDTO>> ExecuteAsync(string userId)
        {
            string whereClause =
               TableQuery.CombineFilters(
               TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, StaticPartitionKeys.PersonalUsagePerWeekdayReport),
               TableOperators.And,
               TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, $"{userId}|Mon"), TableOperators.Or,
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, $"{userId}|Tue"), TableOperators.Or,
                            TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, $"{userId}|Wed"), TableOperators.Or,
                            TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, $"{userId}|Thu"), TableOperators.Or,
                                TableQuery.CombineFilters(
                                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, $"{userId}|Fri"), TableOperators.Or,
                                    TableQuery.CombineFilters(
                                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, $"{userId}|Sat"), TableOperators.Or,
                                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, $"{userId}|Sun")
                                        )
                                    )
                                )
                            )
                        )
                    )
               );

            var result = await this.storageAccessService.QueryTableAsync<PersonalUsagePerWeekdayTableEntity>(TableNames.Reports, whereClause);

            // add missing weekdays
            result.AddRange(orderedWeekdays
                .Where(o => result.Any(r => r.WeekDay == o.Value) == false)
                .Select(o => new PersonalUsagePerWeekdayTableEntity() { WeekDay = o.Value }));

            var query = from r in result
                        join w in orderedWeekdays on r.WeekDay equals w.Value
                        orderby w.Key
                        select r.ToDto();

            return query;
        }
    }
}
