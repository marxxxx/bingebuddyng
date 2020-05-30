using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.User;

namespace BingeBuddyNg.Functions.Services
{
    public class ActivityDistributionService
    {
        private readonly IUserRepository userRepository;
        private readonly IActivityRepository activityRepository;

        public ActivityDistributionService(IUserRepository userRepository, IActivityRepository activityRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        public async Task DistributeActivitiesAsync(User currentUser, Activity activity)
        {
            IEnumerable<string> userIds;

            if (activity.ActivityType == ActivityType.Registration)
            {
                userIds = await this.userRepository.GetAllUserIdsAsync();
            }
            else
            {
                // get friends of this user who didn't mute themselves from him
                userIds = currentUser.GetVisibleFriendUserIds(true);
            }

            await this.activityRepository.DistributeActivityAsync(userIds, activity);
        }

    }
}
