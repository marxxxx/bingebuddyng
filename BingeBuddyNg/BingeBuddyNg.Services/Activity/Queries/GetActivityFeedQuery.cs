using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;
using MediatR;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly IUserRepository userRepository;
        private readonly IActivityRepository activityRepository;
        private readonly IUserStatsRepository userStatsRepository;
        private readonly ICacheService cacheService;

        public GetActivityFeedQueryHandler(
            IUserRepository userRepository,
            IActivityRepository activityRepository,
            IUserStatsRepository userStatsRepository,
            ICacheService cacheService)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.userStatsRepository = userStatsRepository ?? throw new ArgumentNullException(nameof(userStatsRepository));
            this.cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<PagedQueryResult<ActivityStatsDTO>> Handle(GetActivityFeedQuery request, CancellationToken cancellationToken)
        {
            PagedQueryResult<ActivityStatsDTO> result = null;

            if (request.ContinuationToken == null)
            {
                result = await cacheService.GetOrCreateAsync(
                    activityRepository.GetActivityCacheKey(request.UserId),
                    () => HandleGetActivityFeedQuery(request),
                    TimeSpan.FromMinutes(1));
            }
            else
            {
                result = await HandleGetActivityFeedQuery(request);
            }

            return result;
        }

        private async Task<PagedQueryResult<ActivityStatsDTO>> HandleGetActivityFeedQuery(GetActivityFeedQuery request)
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
