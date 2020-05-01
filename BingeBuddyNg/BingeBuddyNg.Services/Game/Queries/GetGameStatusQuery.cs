using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Game.Queries
{
    public class GetGameStatusQuery : IRequest<GameStatusDTO>
    {
        public GetGameStatusQuery(Guid gameId)
        {
            GameId = gameId;
        }

        public Guid GameId { get; }

        public override string ToString()
        {
            return $"{{{nameof(GameId)}={GameId.ToString()}}}";
        }
    }

    public class GetGameStatusQueryHandler : IRequestHandler<GetGameStatusQuery, GameStatusDTO>
    {
        private readonly IGameManager manager;

        public GetGameStatusQueryHandler(IGameManager manager)
        {
            this.manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public async Task<GameStatusDTO> Handle(GetGameStatusQuery request, CancellationToken cancellationToken)
        {
            var game = this.manager.GetGame(request.GameId);
            return game.ToDto();
        }
    }
}
