using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Activity.Commands;
using BingeBuddyNg.Services.Activity.Querys;
using BingeBuddyNg.Services.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        public IIdentityService IdentityService { get; }
        public IMediator Mediator { get; }

        public ActivityController(
            IIdentityService identityService,
            IMediator mediator)
        {
            this.IdentityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }


        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<ActivityDTO>>> GetActivitysForMap()
        {
            var userId = this.IdentityService.GetCurrentUserId();
            var result = await this.Mediator.Send(new GetActivitysForMapQuery(userId));
            return result;
        }

        [HttpGet("[action]")]
        public async Task<PagedQueryResult<ActivityStatsDTO>> GetActivityFeed(string activityId, string continuationToken)
        {
            var userId = this.IdentityService.GetCurrentUserId();
            var result = await this.Mediator.Send(new GetActivityFeedQuery(userId, activityId, continuationToken));
            return result;
        }

        [HttpGet("[action]/{userId}")]
        public async Task<ActionResult<List<ActivityAggregationDTO>>> GetActivityAggregation(string userId)
        {
            var result = await this.Mediator.Send(new GetDrinkActivityAggregationQuery(userId));
            return result;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> AddMessageActivity([FromBody] AddMessageActivityDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userId = this.IdentityService.GetCurrentUserId();
            await this.Mediator.Send(new AddMessageActivityCommand(userId, request.Message, request.Location, request.Venue));
            return Ok();
        }


        [HttpPost("[action]")]
        public async Task<ActionResult> AddDrinkActivity([FromBody] AddDrinkActivityDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userId = this.IdentityService.GetCurrentUserId();
            var command = new AddDrinkActivityCommand(userId, request.DrinkId, request.DrinkType, request.DrinkName, request.AlcPrc, request.Volume, request.Location, request.Venue);
            await this.Mediator.Send(command);

            return Ok();
        }

        [HttpPost("[action]/{lat}/{lng}")]
        public async Task<ActionResult> AddImageActivity(IFormFile file, double? lat, double? lng)
        {
            if (file == null)
            {
                return BadRequest();
            }

            var userId = this.IdentityService.GetCurrentUserId();

            using (var stream = file.OpenReadStream())
            {
                await this.Mediator.Send(new AddImageActivityCommand(userId, stream, file.FileName, lat, lng));
            }

            return Ok();
        }

        [HttpPost("[action]")]
        public async Task AddReaction([FromBody]AddReactionDTO reaction)
        {
            string userId = this.IdentityService.GetCurrentUserId();
            await this.Mediator.Send(new AddReactionCommand(userId, reaction.Type, reaction.ActivityId, reaction.Comment));
        }


        [HttpDelete("{id}")]
        public async Task DeleteActivity(string id)
        {
            string userId = this.IdentityService.GetCurrentUserId();
            await this.Mediator.Send(new DeleteActivityCommand(userId, id));
        }

    }
}
