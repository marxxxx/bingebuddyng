using BingeBuddyNg.Services;
using BingeBuddyNg.Services.Game;
using BingeBuddyNg.Services.Game.Commands;
using BingeBuddyNg.Services.Game.Queries;
using BingeBuddyNg.Services.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IIdentityService identityService;
        private readonly IMediator mediator;

        public GameController(IIdentityService identityService, IMediator mediator)
        {
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("{gameId}")]
        public async Task<GameDTO> GetGame(Guid gameId)
        {
            var query = new GetGameQuery(gameId);
            var result = await this.mediator.Send(query);
            return result;
        }

        [HttpPost("start")]
        public async Task<StartGameResultDTO> StartGame(StartGameDTO game)
        {
            var userId = this.identityService.GetCurrentUserId();
            var command = new StartGameCommand(userId, game.Title, game.PlayerUserIds);
            var result = await this.mediator.Send(command);
            return result;
        }

        [HttpPost("event")]
        public async Task AddGameEvent(AddGameEventDTO gameEvent)
        {
            var userId = this.identityService.GetCurrentUserId();
            var command = new AddGameEventCommand(gameEvent.GameId, userId, gameEvent.Count);
            await this.mediator.Send(command);
        }
    }
}
