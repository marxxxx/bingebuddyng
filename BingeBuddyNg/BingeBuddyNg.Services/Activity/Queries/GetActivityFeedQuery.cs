using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Statistics;
using MediatR;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace BingeBuddyNg.Services.Activity.Querys
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
        private readonly IActivityRepository activityRepository;
        private readonly IUserStatsRepository userStatsRepository;
        
        public GetActivityFeedQueryHandler(
            IActivityRepository activityRepository,
            IUserStatsRepository userStatsRepository,
            ICacheService cacheService)
        {
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.userStatsRepository = userStatsRepository ?? throw new ArgumentNullException(nameof(userStatsRepository));
        }

        public async Task<PagedQueryResult<ActivityStatsDTO>> Handle(GetActivityFeedQuery request, CancellationToken cancellationToken)
        {
            var args = new GetActivityFilterArgs() { UserId = request.UserId, ContinuationToken = request.ContinuationToken, StartActivityId = request.StartActivityId };
            var activities = await this.activityRepository.GetActivityFeedAsync(args);

            var userIds = activities.ResultPage.Select(a => a.UserId).Distinct();
            var userStats = await this.userStatsRepository.GetStatisticsAsync(userIds);

            var result = activities.ResultPage.Select(a => new ActivityStatsDTO(a.ToDto(), userStats.First(u => u.UserId == a.UserId).ToDto())).ToList();
            return new PagedQueryResult<ActivityStatsDTO>(result, activities.ContinuationToken);
        }
    }
}
