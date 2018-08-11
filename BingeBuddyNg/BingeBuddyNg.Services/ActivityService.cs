using BingeBuddyNg.Services.DTO;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services
{
    public class ActivityService : IActivityService
    {
        public IIdentityService IdentityService { get; }
        public IUserRepository UserRepository { get; }
        public IActivityRepository ActivityRepository { get; }

        public ActivityService(IIdentityService identityService,  
            IUserRepository userRepository,
            IActivityRepository activityRepository)
        {
            this.IdentityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }
                

        public async Task AddMessageActivityAsync(AddMessageActivityDTO messageActivity)
        {
            var userId = this.IdentityService.GetCurrentUserId();
            var user = await this.UserRepository.GetUserAsync(userId);

            var activity = new Activity(ActivityType.Message, DateTime.UtcNow, messageActivity.Location, userId, user.Name, user.ProfileImageUrl, messageActivity.Message);

            await this.ActivityRepository.AddActivityAsync(activity);
        }
    }
}
