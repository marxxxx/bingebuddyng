using BingeBuddyNg.Api.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Statistics.Querys
{
    public class StatisticsQueryHandler : IRequestHandler<GetStatisticHistoryForUserQuery, IEnumerable<UserStatisticHistoryDTO>>
    {
        public IUserStatsHistoryRepository UserStatsHistoryRepository { get; }


        public StatisticsQueryHandler(IUserStatsHistoryRepository userStatsHistoryRepository)
        {
            UserStatsHistoryRepository = userStatsHistoryRepository ?? throw new ArgumentNullException(nameof(userStatsHistoryRepository));
        }

        public async Task<IEnumerable<UserStatisticHistoryDTO>> Handle(GetStatisticHistoryForUserQuery request, CancellationToken cancellationToken)
        {
            var history = await UserStatsHistoryRepository.GetStatisticHistoryForUserAsync(request.UserId);
            var result = history.Select(h => new UserStatisticHistoryDTO(h.Timestamp, h.CurrentAlcLevel)).ToList();
            return result;
        }
    }
}
