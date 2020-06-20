using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Statistics;
using BingeBuddyNg.Services.Ranking;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.User.Queries;
using MediatR;

namespace BingeBuddyNg.Core.Ranking.Queries
{
    public class GetDrinksRankingQuery : IRequest<List<UserRankingDTO>>
    {
    }

    public class GetDrinksRankingQueryHandler :
       IRequestHandler<GetDrinksRankingQuery, List<UserRankingDTO>>
    {
        private readonly ISearchUsersQuery getUsersQuery;
        private readonly IUserStatsRepository userStatsRepository;

        public GetDrinksRankingQueryHandler(ISearchUsersQuery getUsersQuery, IUserStatsRepository userStatsRepository)
        {
            this.getUsersQuery = getUsersQuery ?? throw new ArgumentNullException(nameof(getUsersQuery));
            this.userStatsRepository = userStatsRepository ?? throw new ArgumentNullException(nameof(userStatsRepository));
        }

        public async Task<List<UserRankingDTO>> Handle(GetDrinksRankingQuery request, CancellationToken cancellationToken)
        {
            var userStats = await this.userStatsRepository.GetRankingStatisticsAsync();
            var userIds = userStats.Select(u => u.UserId).Distinct();
            var users = await this.getUsersQuery.ExecuteAsync(userIds);

            var result = userStats.Select(s => new UserRankingDTO(users.Select(u => new UserInfoDTO(u.Id, u.Name)).First(u => u.UserId == s.UserId), s.ToDto())).ToList();
            return result;
        }
    }
}
