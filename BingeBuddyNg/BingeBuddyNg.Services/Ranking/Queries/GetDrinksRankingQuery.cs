using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Ranking.DTO;
using BingeBuddyNg.Core.Statistics;
using BingeBuddyNg.Core.User.DTO;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.User.Queries;
using Microsoft.WindowsAzure.Storage.Table;
using static BingeBuddyNg.Shared.Constants;
using BingeBuddyNg.Core.User;
using MediatR;
using System.Threading;

namespace BingeBuddyNg.Core.Ranking.Queries
{
    public class GetDrinksRankingQuery : IRequest<List<UserRankingDTO>>
    {
    }

    public class GetDrinksRankingQueryHandler : IRequestHandler<GetDrinksRankingQuery, List<UserRankingDTO>>
    {
        private readonly IUserRepository userRepository;
        private readonly IStorageAccessService storageAccessService;

        public GetDrinksRankingQueryHandler(IUserRepository userRepository, IStorageAccessService storageAccessService)
        {
            this.userRepository = userRepository;
            this.storageAccessService = storageAccessService;
        }

        public async Task<List<UserRankingDTO>> Handle(GetDrinksRankingQuery request, CancellationToken cancellationToken)
        {
            string whereClause = TableQuery.GenerateFilterConditionForInt(nameof(UserStatsTableEntity.TotalDrinksLastMonth), QueryComparisons.GreaterThan, 0);
            var queryResult = await storageAccessService.QueryTableAsync<UserStatsTableEntity>(TableNames.UserStats, whereClause);

            var userStats = queryResult.OrderByDescending(r => r.TotalDrinksLastMonth)
                .Select(r => new UserStatistics(r.RowKey, r.CurrentAlcoholization, r.CurrentNightDrinks, r.Score, r.TotalDrinksLastMonth))
                .ToList();

            var userIds = userStats.Select(u => u.UserId).Distinct();
            var users = await this.userRepository.SearchUsersAsync(userIds);

            var result = userStats.Select(s => new UserRankingDTO(users.Select(u => new UserInfoDTO(u.Id, u.Name)).First(u => u.UserId == s.UserId), s.ToDto())).ToList();
            return result;
        }
    }
}
