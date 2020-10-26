using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Statistics.DTO;
using BingeBuddyNg.Core.Infrastructure;
using Microsoft.WindowsAzure.Storage.Table;
using static BingeBuddyNg.Shared.Constants;
using MediatR;
using System.Threading;

namespace BingeBuddyNg.Core.Statistics.Queries
{
    public class GetPersonalUsagePerWeekdayQuery : IRequest<IEnumerable<PersonalUsagePerWeekdayDTO>>
    {
        public GetPersonalUsagePerWeekdayQuery(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        public string UserId { get; }
    }

    public class GetPersonalUsagePerWeekdayQueryHandler : IRequestHandler<GetPersonalUsagePerWeekdayQuery, IEnumerable<PersonalUsagePerWeekdayDTO>>
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

        public GetPersonalUsagePerWeekdayQueryHandler(IStorageAccessService storageAccessService)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public async Task<IEnumerable<PersonalUsagePerWeekdayDTO>> Handle(GetPersonalUsagePerWeekdayQuery request, CancellationToken cancellationToken)
        {
            string whereClause =
               TableQuery.CombineFilters(
               TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, StaticPartitionKeys.PersonalUsagePerWeekdayReport),
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
