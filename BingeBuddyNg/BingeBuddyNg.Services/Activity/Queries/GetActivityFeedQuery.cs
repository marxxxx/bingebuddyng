using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity.DTO;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.Statistics;
using BingeBuddyNg.Core.Statistics.Queries;
using BingeBuddyNg.Shared;
using MediatR;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace BingeBuddyNg.Core.Activity.Queries
{
    [DebuggerStepThrough]
    public class GetActivityFeedQuery : IRequest<PagedQueryResult<ActivityStatsDTO>>
    {
        public GetActivityFeedQuery(string userId, string startActivityId = null, string continuationToken = null)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrEmpty(startActivityId) && startActivityId != "null")
            {
                this.StartActivityId = startActivityId;
            }
            if (string.IsNullOrEmpty(continuationToken) == false)
            {
                this.ContinuationToken = JsonConvert.DeserializeObject<TableContinuationToken>(continuationToken);
            }
        }

        public string UserId { get; }
        public string StartActivityId { get; }
        public TableContinuationToken ContinuationToken { get; }
    }

    public class GetActivityFeedQueryHandler : IRequestHandler<GetActivityFeedQuery, PagedQueryResult<ActivityStatsDTO>>
    {
        private readonly GetStatisticsQuery getStatisticsQuery;
        private readonly IStorageAccessService storageAccessService;
        
        public GetActivityFeedQueryHandler(
            IStorageAccessService storageAccessService,
            GetStatisticsQuery getStatisticsQuery)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
            this.getStatisticsQuery = getStatisticsQuery ?? throw new ArgumentNullException(nameof(getStatisticsQuery));
        }

        public async Task<PagedQueryResult<ActivityStatsDTO>> Handle(GetActivityFeedQuery request, CancellationToken cancellationToken)
        {
            var activities = await this.GetActivityFeedAsync(request.UserId, request.ContinuationToken, request.StartActivityId);

            var userIds = activities.ResultPage.Select(a => a.UserId).Distinct();
            var userStats = await this.getStatisticsQuery.ExecuteAsync(userIds);

            var result = activities.ResultPage.Select(a => new ActivityStatsDTO(a, userStats.First(u => u.UserId == a.UserId).ToDto())).ToList();
            return new PagedQueryResult<ActivityStatsDTO>(result, activities.ContinuationToken);
        }

        private async Task<PagedQueryResult<ActivityDTO>> GetActivityFeedAsync(string userId, TableContinuationToken continuationToken, string startActivityId)
        {
            var result = await storageAccessService.QueryTableAsync<JsonTableEntity<ActivityDTO>>(Constants.TableNames.ActivityUserFeed, userId, startActivityId, 30, continuationToken);

            var activities = result.ResultPage.Select(r => r.Entity).ToList();
            return new PagedQueryResult<ActivityDTO>(activities, result.ContinuationToken);
        }
    }
}
