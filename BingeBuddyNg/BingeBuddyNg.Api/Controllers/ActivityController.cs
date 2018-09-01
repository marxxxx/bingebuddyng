using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.DTO;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
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
        public IActivityService ActivityService { get; }
        public IActivityRepository ActivityRepository { get; }

        public ActivityController(IActivityService activityService, IActivityRepository activityRepository)
        {
            this.ActivityService = activityService ?? throw new ArgumentNullException(nameof(activityService));
            this.ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }
        
        
        [HttpGet("{onlyWithLocation}")]
        public async Task<ActionResult<List<Activity>>> Get(bool onlyWithLocation)
        {
            var result = await this.ActivityRepository.GetActivitysAsync(new GetActivityFilterArgs(onlyWithLocation));
            return result;
        }

        [HttpGet("[action]")]
        public async Task<List<ActivityStatsDTO>> GetActivityFeed()
        {
            var result = await this.ActivityService.GetActivitiesAsync();
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

    }
}
