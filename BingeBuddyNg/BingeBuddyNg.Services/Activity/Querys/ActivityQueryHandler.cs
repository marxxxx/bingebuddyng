using BingeBuddyNg.Services.Drink;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Activity.Querys
{
    public class ActivityQueryHandler :
        IRequestHandler<GetActivityFeedQuery, PagedQueryResult<ActivityStatsDTO>>,
        IRequestHandler<GetDrinkActivityAggregationQuery, List<ActivityAggregationDTO>>,
        IRequestHandler<GetActivitysForMapQuery, List<ActivityDTO>>
    {

        public IUserRepository UserRepository { get; }
        public IActivityRepository ActivityRepository { get; }
        public IUserStatsRepository UserStatsRepository { get; }
        public ICacheService CacheService { get; }

        public ActivityQueryHandler(IUserRepository userRepository, 
            IActivityRepository activityRepository, 
            IUserStatsRepository userStatsRepository,
            ICacheService cacheService)
        {
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            UserStatsRepository = userStatsRepository ?? throw new ArgumentNullException(nameof(userStatsRepository));
            CacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<PagedQueryResult<ActivityStatsDTO>> Handle(GetActivityFeedQuery request, CancellationToken cancellationToken)
        {
            PagedQueryResult<ActivityStatsDTO> result = null;

            if (request.ContinuationToken == null)
            {
                result = await CacheService.GetOrCreateAsync(
                    ActivityRepository.GetActivityCacheKey(request.UserId), 
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
            var callingUser = await this.UserRepository.FindUserAsync(request.UserId);

            var visibleUserIds = callingUser.GetVisibleFriendUserIds();
            var args = new GetActivityFilterArgs(request.UserId, visibleUserIds, request.ContinuationToken) { StartActivityId = request.StartActivityId };
            var activities = await this.ActivityRepository.GetActivityFeedAsync(args);

            var userIds = activities.ResultPage.Select(a => a.UserId).Distinct();
            var userStats = await this.UserStatsRepository.GetStatisticsAsync(userIds);

            var result = activities.ResultPage.Select(a => new ActivityStatsDTO(a.ToDto(), userStats.First(u => u.UserId == a.UserId).ToDto())).ToList();
            return new PagedQueryResult<ActivityStatsDTO>(result, activities.ContinuationToken);
        }

        public async Task<List<ActivityAggregationDTO>> Handle(GetDrinkActivityAggregationQuery request, CancellationToken cancellationToken)
        {
            var startTime = DateTime.UtcNow.AddDays(-30).Date;

            var result = await this.ActivityRepository.GetActivitysForUserAsync(request.UserId, startTime, ActivityType.Drink);

            var groupedByDay = result.GroupBy(t => t.Timestamp.Date)
                .OrderBy(t => t.Key)
                .Select(t => new ActivityAggregationDTO()
                {
                    Count = t.Count(),
                    CountBeer = t.Count(d => d.DrinkType == DrinkType.Beer),
                    CountWine = t.Count(d => d.DrinkType == DrinkType.Wine),
                    CountShots = t.Count(d => d.DrinkType == DrinkType.Shot),
                    CountAnti = t.Count(d => d.DrinkType == DrinkType.Anti),
                    CountAlc = t.Count(d => d.DrinkType != DrinkType.Anti),
                    Day = t.Key
                })
                .ToList();

            // now fill holes of last 30 days
            for (int i = -30; i < 0; i++)
            {
                var date = DateTime.UtcNow.AddDays(i).Date;
                var hasData = groupedByDay.Any(d => d.Day == date);
                if (hasData == false)
                {
                    groupedByDay.Add(new ActivityAggregationDTO(date));
                }
            }

            var sortedResult = groupedByDay.OrderBy(d => d.Day).ToList();

            return sortedResult;
        }

        public async Task<List<ActivityDTO>> Handle(GetActivitysForMapQuery request, CancellationToken cancellationToken)
        {
            var result = await this.ActivityRepository.GetActivityFeedAsync(new GetActivityFilterArgs(request.UserId, ActivityFilterOptions.WithLocation, pageSize: 50));
            return result.ResultPage?.Select(a=> a.ToDto()).ToList();
        }
    }
}
