using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using BingeBuddyNg.Services.Models.Venue;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BingeBuddyNg.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class VenueController : Controller
    {
        public IVenueService FourSquareService { get; }
        public IIdentityService IdentityService { get; set; }
        public IUserRepository UserRepository { get; set; }
        public IActivityRepository ActivityRepository { get; set; }

        public VenueController(IVenueService fourSquareService, IIdentityService identityService, IUserRepository userRepository, IActivityRepository activityRepository)
        {
            FourSquareService = fourSquareService ?? throw new ArgumentNullException(nameof(fourSquareService));
            IdentityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }


        [HttpGet]
        public async Task<IEnumerable<VenueModel>> SearchVenues(float latitude, float longitude)
        {
            var venues = await this.FourSquareService.SearchVenuesAsync(latitude, longitude);
            return venues;
        }

        [HttpPost("[action]")]
        public async Task UpdateCurrentVenue([FromBody]VenueModel venue)
        {
            var userId = this.IdentityService.GetCurrentUserId();

            var user = await this.UserRepository.FindUserAsync(userId);
            if (user.CurrentVenue != venue)
            {
                user.CurrentVenue = venue;

                await this.UserRepository.UpdateUserAsync(user);

                var notificationActivity = Activity.CreateNotificationActivity(DateTime.UtcNow, user.Id, user.Name,
                           $"Ich bin jetzt hier eingekehrt: {venue.Name}");
                await this.ActivityRepository.AddActivityAsync(notificationActivity);
            }
        }

        [HttpPost("[action]")]
        public async Task ResetCurrentVenue()
        {
            var userId = this.IdentityService.GetCurrentUserId();

            var user = await this.UserRepository.FindUserAsync(userId);
            user.CurrentVenue = null;

            await this.UserRepository.UpdateUserAsync(user);

            var notificationActivity = Activity.CreateNotificationActivity(DateTime.UtcNow, user.Id, user.Name,
                $"Ich bin jetzt im nirgendwo.");
            await this.ActivityRepository.AddActivityAsync(notificationActivity);
        }
    }
}
