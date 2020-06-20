using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.User.Queries;
using MediatR;

namespace BingeBuddyNg.Services.Game.Queries
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
        private readonly IGameManager manager;
        private readonly ISearchUsersQuery getUsersQuery;

        public GetGameQueryHandler(IGameManager manager, ISearchUsersQuery getUsersQuery)
        {
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
            this.getUsersQuery = getUsersQuery ?? throw new ArgumentNullException(nameof(getUsersQuery));
        }

        public async Task<GameDTO> Handle(GetGameQuery request, CancellationToken cancellationToken)
        {
            var game = this.manager.GetGame(request.GameId);
            var users = await this.getUsersQuery.ExecuteAsync(game.PlayerUserIds);
            return game.ToDto(users.Select(u=>u.ToUserInfoDTO()));
        }
    }
}
