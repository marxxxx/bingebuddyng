using BingeBuddyNg.Services.DTO;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Messages;
using BingeBuddyNg.Services.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services
{
    public class ActivityService : IActivityService
    {
        public IIdentityService IdentityService { get; }
        public IUserRepository UserRepository { get; }
        public IActivityRepository ActivityRepository { get; }
        public StorageAccessService StorageAccessService { get; }

        public ActivityService(IIdentityService identityService,  
            IUserRepository userRepository,
            IActivityRepository activityRepository, 
            StorageAccessService storageAccessService)
        {
            this.IdentityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.StorageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }
                

        public async Task AddMessageActivityAsync(AddMessageActivityDTO messageActivity)
        {
            var userId = this.IdentityService.GetCurrentUserId();
            var user = await this.UserRepository.GetUserAsync(userId);

            var activity = new Activity(ActivityType.Message, DateTime.UtcNow, messageActivity.Location, userId, user.Name, user.ProfileImageUrl, messageActivity.Message);

            var savedActivity = await this.ActivityRepository.AddActivityAsync(activity);

            var queueClient = this.StorageAccessService.GetQueueReference(Constants.ActivityAddedQueueName);
            var message = new ActivityAddedMessage(savedActivity);
            await queueClient.AddMessageAsync(new Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage(JsonConvert.SerializeObject(message)));
        }

        public async Task<List<ActivityAggregationDTO>> GetDrinkActivityAggregationAsync()
        {
            string userId = this.IdentityService.GetCurrentUserId();

            var startTime = DateTime.UtcNow.AddDays(-30).Date;
            // TODO: Change to correct type!
            var result = await this.ActivityRepository.GetActivitysForUser(userId, startTime, ActivityType.Message);

            var groupedByDay = result.GroupBy(t => t.Timestamp.Date)
                .OrderBy(t => t.Key)
                .Select(t => new ActivityAggregationDTO() { Count = t.Count(), Day = t.Key })
                .ToList();

            // now fill holes of last 30 days
            for(int i=-30; i<0; i++)
            {
                var date = DateTime.UtcNow.AddDays(i).Date;
                var hasData = groupedByDay.Any(d => d.Day == date);
                if (hasData == false)
                {
                    groupedByDay.Add(new ActivityAggregationDTO(date, 0));
                }
            }

            var sortedResult = groupedByDay.OrderBy(d => d.Day).ToList();

            return sortedResult;
        }
    }
}
