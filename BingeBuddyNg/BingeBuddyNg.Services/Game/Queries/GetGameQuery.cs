using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Services.Game.DTO;
using BingeBuddyNg.Services.User.Queries;
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
        private readonly SearchUsersQuery getUsersQuery;

        public GetGameQueryHandler(GameRepository manager, SearchUsersQuery getUsersQuery)
        {
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
            this.getUsersQuery = getUsersQuery ?? throw new ArgumentNullException(nameof(getUsersQuery));
        }

        public async Task<GameDTO> Handle(GetGameQuery request, CancellationToken cancellationToken)
        {
            var game = this.manager.Get(request.GameId);
            var users = await this.getUsersQuery.ExecuteAsync(game.PlayerUserIds);
            return game.ToDto(users.Select(u=>u.ToUserInfoDTO()));
        }
    }
}
