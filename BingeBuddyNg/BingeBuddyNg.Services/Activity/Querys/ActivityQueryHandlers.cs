﻿using BingeBuddyNg.Services.Drink;
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
    public class ActivityQueryHandlers 
        : IRequestHandler<GetActivityFeedQuery, PagedQueryResult<ActivityStatsDTO>>,
        IRequestHandler<GetDrinkActivityAggregationQuery, List<ActivityAggregationDTO>>
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
    }
}
