using System;
using System.Collections.Generic;
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
using static BingeBuddyNg.Shared.Constants;

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

        public GetActivityFeedQueryHandler(
            IStorageAccessService storageAccessService)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public async Task<PagedQueryResult<ActivityStatsDTO>> Handle(GetActivityFeedQuery request, CancellationToken cancellationToken)
        {
            var activities = await this.GetActivityFeedAsync(request.UserId, request.ContinuationToken, request.StartActivityId);

            var userIds = activities.ResultPage.Select(a => a.UserId).Distinct();
            var userStats = await this.GetStatisticsAsync(userIds);

            var result = activities.ResultPage.Select(a => new ActivityStatsDTO(a, userStats.First(u => u.UserId == a.UserId).ToDto())).ToList();
            return new PagedQueryResult<ActivityStatsDTO>(result, activities.ContinuationToken);
        }

        private async Task<PagedQueryResult<ActivityDTO>> GetActivityFeedAsync(string userId, TableContinuationToken continuationToken, string startActivityId)
        {
            var result = await storageAccessService.QueryTableAsync<JsonTableEntity<ActivityEntity>>(Constants.TableNames.ActivityUserFeed, userId, startActivityId, 30, continuationToken);

            var activities = result.ResultPage.Select(r => r.Entity.ToDto()).ToList();
            return new PagedQueryResult<ActivityDTO>(activities, result.ContinuationToken);
        }

        private async Task<List<UserStatistics>> GetStatisticsAsync(IEnumerable<string> userId)
        {
            var tasks = userId.Select(u => GetStatisticsAsync(u));
            var result = await Task.WhenAll(tasks);
            return result.ToList();
        }

        private async Task<UserStatistics> GetStatisticsAsync(string userId)
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