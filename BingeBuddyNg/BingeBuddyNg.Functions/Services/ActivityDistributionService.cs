using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity.Commands;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Core.User.Domain;
using BingeBuddyNg.Core.User.Queries;
using BingeBuddyNg.Services.Activity.Persistence;

namespace BingeBuddyNg.Functions.Services
{
    public class ActivityDistributionService
    {
        private readonly GetAllUserIdsQuery getAllUserIdsQuery;
        private readonly DistributeActivityToPersonalizedFeedCommand distributeActivityToPersonalizedFeedCommand;

        public ActivityDistributionService(GetAllUserIdsQuery getAllUserIdsQuery, DistributeActivityToPersonalizedFeedCommand distributeActivityToPersonalizedFeedCommand)
        {
            this.getAllUserIdsQuery = getAllUserIdsQuery;
            this.distributeActivityToPersonalizedFeedCommand = distributeActivityToPersonalizedFeedCommand;
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

            await this.distributeActivityToPersonalizedFeedCommand.ExecuteAsync(userIds, activity);
        }
    }
}
