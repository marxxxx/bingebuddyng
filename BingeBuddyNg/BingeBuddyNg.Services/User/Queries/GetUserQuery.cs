using System;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Statistics;
using BingeBuddyNg.Core.User.DTO;
using MediatR;

namespace BingeBuddyNg.Core.User.Queries
{
    public class GetUserQuery : IRequest<UserDTO>
    {
        public GetUserQuery(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        public string UserId { get; }
    }

    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDTO>
    {
        private readonly IUserRepository userRepository;
        private readonly StatisticService statisticService;

        public GetUserQueryHandler(IUserRepository userRepository, StatisticService statisticService)
        {
            this.userRepository = userRepository;
            this.statisticService = statisticService;
        }

        public async Task<UserDTO> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserAsync(request.UserId);
            var stats = await statisticService.GetStatisticsAsync(request.UserId);

            return user.ToDto(stats);
        }
    }
}