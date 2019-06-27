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
    public class ActivityQueryHandlers : IRequestHandler<GetActivityFeedQuery, PagedQueryResult<ActivityStatsDTO>>
    {
        public ActivityQueryHandlers(IUserRepository userRepository, IActivityRepository activityRepository, IUserStatsRepository userStatsRepository)
        {
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            UserStatsRepository = userStatsRepository ?? throw new ArgumentNullException(nameof(userStatsRepository));
        }

        public IUserRepository UserRepository { get; }
        public IActivityRepository ActivityRepository { get; }
        public IUserStatsRepository UserStatsRepository { get; }

        public async Task<PagedQueryResult<ActivityStatsDTO>> Handle(GetActivityFeedQuery request, CancellationToken cancellationToken)
        {
            var callingUser = await this.UserRepository.FindUserAsync(request.UserId);

            // TODO: Use Constant for Page Size
            var visibleUserIds = callingUser.GetVisibleFriendUserIds();
            var activities = await this.ActivityRepository.GetActivityFeedAsync(new GetActivityFilterArgs(visibleUserIds, request.ContinuationToken));

            var userIds = activities.ResultPage.Select(a => a.UserId).Distinct();
            var userStats = await this.UserStatsRepository.GetStatisticsAsync(userIds);

            var result = activities.ResultPage.Select(a => new ActivityStatsDTO(a, userStats.First(u => u.UserId == a.UserId))).ToList();
            return new PagedQueryResult<ActivityStatsDTO>(result, activities.ContinuationToken);
        }
    }
}
