using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity.DTO;
using BingeBuddyNg.Core.Activity.Persistence;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.Statistics;
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
        private readonly IStorageAccessService storageAccessService;
        private readonly StatisticService statisticService;

        public GetActivityFeedQueryHandler(
            IStorageAccessService storageAccessService, 
            StatisticService statisticService)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
            this.statisticService = statisticService ?? throw new ArgumentNullException(nameof(statisticService));
        }

        public async Task<PagedQueryResult<ActivityStatsDTO>> Handle(GetActivityFeedQuery request, CancellationToken cancellationToken)
        {
            var activities = await this.GetActivityFeedAsync(request.UserId, request.ContinuationToken, request.StartActivityId);

            var userIds = activities.ResultPage.Select(a => a.UserId).Distinct();
            var userStats = await this.statisticService.GetStatisticsAsync(userIds);

            var result = activities.ResultPage.Select(a => new ActivityStatsDTO(a, userStats.First(u => u.UserId == a.UserId).ToDto())).ToList();
            return new PagedQueryResult<ActivityStatsDTO>(result, activities.ContinuationToken);
        }

        private async Task<PagedQueryResult<ActivityDTO>> GetActivityFeedAsync(string userId, TableContinuationToken continuationToken, string startActivityId)
        {
            var result = await storageAccessService.QueryTableAsync<JsonTableEntity<ActivityEntity>>(Constants.TableNames.ActivityUserFeed, userId, startActivityId, 30, continuationToken);

            var activities = result.ResultPage.Select(r => r.Entity.ToDto()).ToList();
            return new PagedQueryResult<ActivityDTO>(activities, result.ContinuationToken);
        }
    }
}