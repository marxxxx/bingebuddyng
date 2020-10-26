using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.Activity.Persistence;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Core.User.Domain;

namespace BingeBuddyNg.Functions.Services
{
    public class ActivityDistributionService
    {
        private readonly IUserRepository userRepository;
        private readonly IActivityRepository activityRepository;

        public ActivityDistributionService(IUserRepository userRepository, IActivityRepository activityRepository)
        {
            this.userRepository = userRepository;
            this.activityRepository = activityRepository;
        }

        public async Task DistributeActivitiesAsync(User currentUser, ActivityEntity activity)
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

            await this.activityRepository.AddToPersonalizedFeedAsync(userIds, activity);
        }
    }
}
