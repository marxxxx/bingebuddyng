﻿using System;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Game.Commands;
using BingeBuddyNg.Core.Game.DTO;
using BingeBuddyNg.Core.Game.Queries;
using BingeBuddyNg.Core.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IIdentityService identityService;
        private readonly ISender mediator;

        public GameController(IIdentityService identityService, ISender mediator)
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

        [HttpPost("{gameId}/event")]
        public async Task AddGameEvent(Guid gameId, AddGameEventDTO gameEvent)
        {
            var userId = this.identityService.GetCurrentUserId();
            var command = new AddGameEventCommand(gameId, userId, gameEvent.Count);
            await this.mediator.Send(command);
        }
    }
}