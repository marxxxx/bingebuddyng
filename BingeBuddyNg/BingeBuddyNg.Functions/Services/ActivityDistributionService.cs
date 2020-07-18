using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.Activity.Persistence;
using BingeBuddyNg.Core.User.Domain;
using BingeBuddyNg.Core.User.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingeBuddyNg.Functions.Services
{
    public class ActivityDistributionService
    {
        private readonly GetAllUserIdsQuery getAllUserIdsQuery;
        private readonly IActivityRepository activityRepository;

        public ActivityDistributionService(GetAllUserIdsQuery getAllUserIdsQuery, IActivityRepository activityRepository)
        {
            this.getAllUserIdsQuery = getAllUserIdsQuery;
            this.activityRepository = activityRepository;
        }

        public async Task DistributeActivitiesAsync(User currentUser, ActivityEntity activity)
        {
            IEnumerable<string> userIds;

            if (activity.ActivityType == ActivityType.Registration)
            {
                userIds = await this.getAllUserIdsQuery.ExecuteAsync();
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
