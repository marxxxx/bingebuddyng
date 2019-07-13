using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Ranking.Querys
{
    public class RankingQueryHandler :
        IRequestHandler<GetDrinksRankingQuery, List<UserRankingDTO>>,
        IRequestHandler<GetScoreRankingQuery, List<UserRankingDTO>>,
        IRequestHandler<GetVenueRankingQuery, IEnumerable<VenueRankingDTO>>
    {
        public IActivityRepository ActivityRepository { get; }
        public IUserRepository UserRepository { get; }
        public IUserStatsRepository UserStatsRepository { get; }

        public RankingQueryHandler(IActivityRepository activityRepository, IUserRepository userRepository, IUserStatsRepository userStatsRepository)
        {
            ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            UserStatsRepository = userStatsRepository ?? throw new ArgumentNullException(nameof(userStatsRepository));
        }

        public async Task<List<UserRankingDTO>> Handle(GetDrinksRankingQuery request, CancellationToken cancellationToken)
        {
            var userStats = await this.UserStatsRepository.GetRankingStatisticsAsync();
            var userIds = userStats.Select(u => u.UserId).Distinct();
            var users = await this.UserRepository.GetUsersAsync(userIds);

            var result = userStats.Select(s => new UserRankingDTO(users.Select(u=>new UserInfoDTO(u.Id, u.Name)).First(u => u.UserId == s.UserId), Statistics.Converter.ConvertUserStatisticsToDto(s))).ToList();
            return result;
        }

        public async Task<List<UserRankingDTO>> Handle(GetScoreRankingQuery request, CancellationToken cancellationToken)
        {
            var userStats = await this.UserStatsRepository.GetScoreStatisticsAsync();
            var userIds = userStats.Select(u => u.UserId).Distinct();
            var users = await this.UserRepository.GetUsersAsync(userIds);

            var result = userStats.Select(s => new UserRankingDTO(users.Select(u => new UserInfoDTO(u.Id, u.Name)).First(u => u.UserId == s.UserId), Statistics.Converter.ConvertUserStatisticsToDto(s)))
                .OrderByDescending(r => r.Statistics.Score)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<VenueRankingDTO>> Handle(GetVenueRankingQuery request, CancellationToken cancellationToken)
        {
            var activitys = await ActivityRepository.GetActivityFeedAsync(new GetActivityFilterArgs(ActivityFilterOptions.WithVenue, pageSize: 100, activityType: ActivityType.Drink));
            var result = activitys.ResultPage.GroupBy(r => new { r.Venue.Id, r.Venue.Name })
                .Select(r => new VenueRankingDTO(r.Key.Id, r.Key.Name, r.Count()))
                .OrderByDescending(r => r.Count);
            return result;
        }

       
    }
}
