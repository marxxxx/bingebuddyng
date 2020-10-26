using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.Statistics.DTO;
using MediatR;
using Microsoft.WindowsAzure.Storage.Table;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.Statistics.Querys
{
    public class GetStatisticHistoryForUserQuery : IRequest<IEnumerable<UserStatisticHistoryDTO>>
    {
        public GetStatisticHistoryForUserQuery(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        public string UserId { get; }
    }

    public class GetStatisticHistoryForUserQueryHandler : IRequestHandler<GetStatisticHistoryForUserQuery, IEnumerable<UserStatisticHistoryDTO>>
    {
        private readonly IStorageAccessService storageAccessService;

        public GetStatisticHistoryForUserQueryHandler(IStorageAccessService storageAccessService)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public async Task<IEnumerable<UserStatisticHistoryDTO>> Handle(GetStatisticHistoryForUserQuery request, CancellationToken cancellationToken)
        {
            string whereClause =
                TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, request.UserId),
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
