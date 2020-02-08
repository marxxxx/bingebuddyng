using BingeBuddyNg.Services.Infrastructure;
using MediatR;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Statistics.Querys
{
    public class AnalyticReportsQueryHandler : IRequestHandler<GetPersonalUsagePerWeekdayQuery, IEnumerable<PersonalUsagePerWeekdayDTO>>
    {
        private readonly IStorageAccessService storageAccessService;

        private const string ReportsTableName = "reports";

        private const string PersonalUsagePerWeekdayReportPartitionKey = "personalusageperweekdayreport";

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

        public AnalyticReportsQueryHandler(IStorageAccessService storageAccessService)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public async Task<IEnumerable<PersonalUsagePerWeekdayDTO>> Handle(GetPersonalUsagePerWeekdayQuery request, CancellationToken cancellationToken)
        {
            string whereClause =
               TableQuery.CombineFilters(
               TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PersonalUsagePerWeekdayReportPartitionKey),
               TableOperators.And,
               TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, $"{request.UserId}|Mon"), TableOperators.Or,
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, $"{request.UserId}|Tue"), TableOperators.Or,
                            TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, $"{request.UserId}|Wed"), TableOperators.Or,
                            TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, $"{request.UserId}|Thu"), TableOperators.Or,
                                TableQuery.CombineFilters(
                                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, $"{request.UserId}|Fri"), TableOperators.Or,
                                    TableQuery.CombineFilters(
                                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, $"{request.UserId}|Sat"), TableOperators.Or,
                                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, $"{request.UserId}|Sun")
                                        )
                                    )
                                )
                            )
                        )
                    )
               );

            var result = await this.storageAccessService.QueryTableAsync<PersonalUsagePerWeekdayTableEntity>(ReportsTableName, whereClause);

            // add missing weekdays
            result.AddRange(orderedWeekdays
                .Where(o => result.Any(r => r.weekDay == o.Value) == false)
                .Select(o=>new PersonalUsagePerWeekdayTableEntity() { weekDay = o.Value }));

            var query = from r in result
                        join w in orderedWeekdays on r.weekDay equals w.Value
                        orderby w.Key
                        select r.ToDto();

            return query.ToList();
        }
    }
}
