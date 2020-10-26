using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Game.DTO;
using BingeBuddyNg.Core.User;
using MediatR;

namespace BingeBuddyNg.Core.Game.Queries
{
    public class GetGameQuery : IRequest<GameDTO>
    {
        public GetGameQuery(Guid gameId)
        {
            GameId = gameId;
        }

        public Guid GameId { get; }

        public override string ToString()
        {
            return $"{{{nameof(GameId)}={GameId.ToString()}}}";
        }
    }

    public class GetGameQueryHandler : IRequestHandler<GetGameQuery, GameDTO>
    {
        private readonly GameRepository manager;
        private readonly IUserRepository userRepository;

        public GetGameQueryHandler(GameRepository manager, IUserRepository userRepository)
        {
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<GameDTO> Handle(GetGameQuery request, CancellationToken cancellationToken)
        {
            var game = this.manager.Get(request.GameId);
            var users = await this.userRepository.SearchUsersAsync(game.PlayerUserIds);
            return game.ToDto(users.Select(u => u.ToUserInfoDTO()));
        }
    }
}