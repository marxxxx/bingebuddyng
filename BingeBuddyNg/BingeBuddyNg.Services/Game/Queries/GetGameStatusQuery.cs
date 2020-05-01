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
        public Task<GameStatusDTO> Handle(GetGameStatusQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
