using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Ranking.Querys
{
    public class GetScoreRankingQuery : IRequest<List<UserRankingDTO>>
    {
    }

    public class GetScoreRankingQueryHandler : IRequestHandler<GetScoreRankingQuery, List<UserRankingDTO>>
    {
        private readonly IUserRepository userRepository;
        private readonly IUserStatsRepository userStatsRepository;

        public GetScoreRankingQueryHandler(IUserRepository userRepository, IUserStatsRepository userStatsRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.userStatsRepository = userStatsRepository ?? throw new ArgumentNullException(nameof(userStatsRepository));
        }

        public async Task<List<UserRankingDTO>> Handle(GetScoreRankingQuery request, CancellationToken cancellationToken)
        {
            var userStats = await this.userStatsRepository.GetScoreStatisticsAsync();
            var userIds = userStats.Select(u => u.UserId).Distinct();
            var users = await this.userRepository.GetUsersAsync(userIds);

            var result = userStats.Select(s => new UserRankingDTO(users.Select(u => new UserInfoDTO(u.Id, u.Name)).First(u => u.UserId == s.UserId), s.ToDto()))
                .OrderByDescending(r => r.Statistics.Score)
                .ToList();

            return result;
        }
    }

}
