using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Activity.Commands;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.Activity.DTO;
using BingeBuddyNg.Core.Activity.Queries;
using BingeBuddyNg.Core.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IIdentityService identityService;
        private readonly IMediator mediator;
        private readonly GetMasterActivitiesQuery getMasterActivitiesQuery;
        private readonly IActivityRepository activityRepository;

        public ActivityController(
            IIdentityService identityService,
            IMediator mediator,
            GetMasterActivitiesQuery getMasterActivitiesQuery, IActivityRepository activityRepository)
        {
            this.identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.getMasterActivitiesQuery = getMasterActivitiesQuery ?? throw new ArgumentNullException(nameof(getMasterActivitiesQuery));
            this.activityRepository = activityRepository;
        }

        [HttpGet("map")]
        public async Task<ActionResult<IEnumerable<ActivityDTO>>> GetActivitysForMap()
        {
            var args = new ActivityFilterArgs() { FilterOptions = ActivityFilterOptions.WithLocation, PageSize = 50 };

            var result = await this.getMasterActivitiesQuery.ExecuteAsync(args);

            return result.Select(r=>r.ToDto()).ToList();
        }

        [HttpGet("feed")]
        public async Task<PagedQueryResult<ActivityStatsDTO>> GetActivityFeed(string activityId, string continuationToken)
        {
            var userId = this.identityService.GetCurrentUserId();
            var result = await this.mediator.Send(new GetActivityFeedQuery(userId, activityId, continuationToken));
            return result;
        }

        [HttpGet("aggregate/{userId}")]
        public async Task<ActionResult<List<ActivityAggregationDTO>>> GetActivityAggregation(string userId)
        {
            var result = await this.mediator.Send(new GetDrinkActivityAggregationQuery(userId));
            return result;
        }

        [HttpPost("message")]
        public async Task<ActionResult> AddMessageActivity([FromBody] AddMessageActivityDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userId = this.identityService.GetCurrentUserId();
            var activityId = await this.mediator.Send(new AddMessageActivityCommand(userId, request.Message, request.Location, request.Venue));
            return new JsonResult(activityId);
        }

        [HttpPost("drink")]
        public async Task<ActionResult> AddDrinkActivity([FromBody] AddDrinkActivityDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userId = this.identityService.GetCurrentUserId();
            var command = new AddDrinkActivityCommand(userId, request.DrinkId, request.DrinkType, request.DrinkName, request.AlcPrc, request.Volume, request.Location, request.Venue);
            var activityId = await this.mediator.Send(command);

            return new JsonResult(activityId);
        }

        [HttpPost("image/{lat}/{lng}")]
        public async Task<ActionResult> AddImageActivity(IFormFile file, double? lat, double? lng)
        {
            if (file == null)
            {
                return BadRequest();
            }

            var userId = this.identityService.GetCurrentUserId();

            string activityId = null;
            using (var stream = file.OpenReadStream())
            {
                activityId = await this.mediator.Send(new AddImageActivityCommand(userId, stream, file.FileName, lat, lng));
            }

            return new JsonResult(activityId);
        }

        [HttpPost("{activityId}/reaction")]
        public async Task AddReaction(string activityId, [FromBody]AddReactionDTO reaction)
        {
            string userId = this.identityService.GetCurrentUserId();
            await this.mediator.Send(new AddReactionCommand(userId, reaction.Type, activityId, reaction.Comment));
        }

        [HttpDelete("{activityId}")]
        public async Task DeleteActivity(string activityId)
        {
            string userId = this.identityService.GetCurrentUserId();
            await this.mediator.Send(new DeleteActivityCommand(userId, activityId));
        }
    }
}
