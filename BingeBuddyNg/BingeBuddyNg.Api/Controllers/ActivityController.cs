using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        public IIdentityService IdentityService { get; }
        public IActivityService ActivityService { get; }
        public IActivityRepository ActivityRepository { get; }

        public ActivityController(
            IIdentityService identityService,
            IActivityService activityService, IActivityRepository activityRepository)
        {
            this.IdentityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.ActivityService = activityService ?? throw new ArgumentNullException(nameof(activityService));
            this.ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }
        
        
        [HttpGet("{onlyWithLocation}")]
        public async Task<ActionResult<List<Activity>>> Get(bool onlyWithLocation)
        {
            var filterOptions = onlyWithLocation ? ActivityFilterOptions.WithLocation : ActivityFilterOptions.None;
            var result = await this.ActivityRepository.GetActivityFeedAsync(new GetActivityFilterArgs(filterOptions, pageSize: 50));
            return result.ResultPage;
        }

        [HttpGet("[action]")]
        public async Task<PagedQueryResult<ActivityStatsDTO>> GetActivityFeed(string continuationToken)
        {
            var userId = this.IdentityService.GetCurrentUserId();
            TableContinuationToken tableContinuationToken = null;
            if(string.IsNullOrEmpty(continuationToken) == false)
            {
                tableContinuationToken = JsonConvert.DeserializeObject<TableContinuationToken>(continuationToken);
            }
            var result = await this.ActivityService.GetActivityFeedAsync(userId, tableContinuationToken);
            return result;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<List<ActivityAggregationDTO>>> GetActivityAggregation()
        {
            var result = await this.ActivityService.GetDrinkActivityAggregationAsync();
            return result;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> AddMessageActivity([FromBody] AddMessageActivityDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await this.ActivityService.AddMessageActivityAsync(request);
            return Ok();
        }


        [HttpPost("[action]")]
        public async Task<ActionResult> AddDrinkActivity([FromBody] AddDrinkActivityDTO request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            await this.ActivityService.AddDrinkActivityAsync(request);

            return Ok();
        }

        [HttpPost("[action]/{lat}/{lng}")]
        public async Task<ActionResult> AddImageActivity(IFormFile file, double? lat, double? lng)
        {
            if(file == null)
            {
                return BadRequest();
            }

            Location location = null;

            if(lat.GetValueOrDefault() + lng.GetValueOrDefault() > 0)
            {
                location = new Location(lat.Value, lng.Value);
            }

            using (var stream = file.OpenReadStream())
            {
                await this.ActivityService.AddImageActivityAsync(stream, file.FileName, location);
            }

            return Ok();
        }

        [HttpPost("[action]")]
        public async Task AddReaction([FromBody]ReactionDTO reaction)
        {
            await this.ActivityService.AddReactionAsync(reaction);
        }


        [HttpDelete("{id}")]
        public async Task DeleteActivity(string id)
        {
            string userId = this.IdentityService.GetCurrentUserId();
            await this.ActivityRepository.DeleteActivityAsync(userId, id);
        }

    }
}
