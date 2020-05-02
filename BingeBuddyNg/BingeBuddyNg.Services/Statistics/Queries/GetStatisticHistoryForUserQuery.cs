using BingeBuddyNg.Api.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Statistics.Querys
{
    public class GetStatisticHistoryForUserQuery : IRequest<IEnumerable<UserStatisticHistoryDTO>>
    {
        public GetStatisticHistoryForUserQuery(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        public string UserId { get; }
    }

    public class GetStatisticHistoryForUserQueryHandler : IRequestHandler<GetStatisticHistoryForUserQuery, IEnumerable<UserStatisticHistoryDTO>>
    {
        private readonly IUserStatsHistoryRepository userStatsHistoryRepository;

        public GetStatisticHistoryForUserQueryHandler(IUserStatsHistoryRepository userStatsHistoryRepository)
        {
            this.userStatsHistoryRepository = userStatsHistoryRepository ?? throw new ArgumentNullException(nameof(userStatsHistoryRepository));
        }

        public async Task<IEnumerable<UserStatisticHistoryDTO>> Handle(GetStatisticHistoryForUserQuery request, CancellationToken cancellationToken)
        {
            var history = await userStatsHistoryRepository.GetStatisticHistoryForUserAsync(request.UserId);
            var result = history.Select(h => new UserStatisticHistoryDTO(h.Timestamp, h.CurrentAlcLevel)).ToList();
            return result;
        }
    }
}
