using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.User.Queries;
using MediatR;

namespace BingeBuddyNg.Services.Ranking.Querys
{
    public class GetScoreRankingQuery : IRequest<List<UserRankingDTO>>
    {
    }

    public class GetScoreRankingQueryHandler : IRequestHandler<GetScoreRankingQuery, List<UserRankingDTO>>
    {
        private readonly ISearchUsersQuery getAllUsersQuery;
        private readonly IUserStatsRepository userStatsRepository;

        public GetScoreRankingQueryHandler(ISearchUsersQuery getAllUsersQuery, IUserStatsRepository userStatsRepository)
        {
            this.getAllUsersQuery = getAllUsersQuery ?? throw new ArgumentNullException(nameof(getAllUsersQuery));
            this.userStatsRepository = userStatsRepository ?? throw new ArgumentNullException(nameof(userStatsRepository));
        }

        public async Task<List<UserRankingDTO>> Handle(GetScoreRankingQuery request, CancellationToken cancellationToken)
        {
            var userStats = await this.userStatsRepository.GetScoreStatisticsAsync();
            var userIds = userStats.Select(u => u.UserId).Distinct();
            var users = await this.getAllUsersQuery.ExecuteAsync(userIds);

            var result = userStats.Select(s => new UserRankingDTO(users.First(u => u.Id == s.UserId).ToUserInfoDTO(), s.ToDto()))
                .OrderByDescending(r => r.Statistics.Score)
                .ToList();

            return result;
        }
    }

}
