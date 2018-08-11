﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.DTO;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using Microsoft.AspNetCore.Authorization;
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
        
        
        [HttpGet]
        public async Task<ActionResult<List<Activity>>> Get()
        {
            var result = await this.ActivityRepository.GetActivitysAsync();
            return result;
        }

        [HttpPost]
        public async Task Post([FromBody] AddMessageActivityDTO request)
        {
            await this.ActivityService.AddMessageActivityAsync(request);
        }

    }
}
