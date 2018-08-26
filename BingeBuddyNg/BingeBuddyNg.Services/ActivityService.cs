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
        public IUserStatsRepository UserStatsRepository { get; }
        public StorageAccessService StorageAccessService { get; }

        public ActivityService(IIdentityService identityService,  
            IUserRepository userRepository,
            IActivityRepository activityRepository, 
            IUserStatsRepository userStatsRepository,
            StorageAccessService storageAccessService)
        {
            this.IdentityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.UserStatsRepository = userStatsRepository ?? throw new ArgumentNullException(nameof(userStatsRepository));
            this.StorageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }


        public async Task AddMessageActivityAsync(AddMessageActivityDTO addedActivity)
        {
            var userId = this.IdentityService.GetCurrentUserId();
            var user = await this.UserRepository.GetUserAsync(userId);

            var activity = Activity.CreateMessageActivity(DateTime.UtcNow, addedActivity.Location, userId, user.Name, user.ProfileImageUrl, addedActivity.Message);

            var savedActivity = await this.ActivityRepository.AddActivityAsync(activity);
            await AddToActivityAddedQueueAsync(savedActivity);
        }

        public async Task AddDrinkActivityAsync(AddDrinkActivityDTO addedActivity)
        {
            var userId = this.IdentityService.GetCurrentUserId();
            var user = await this.UserRepository.GetUserAsync(userId);

            var activity = Activity.CreateDrinkActivity(DateTime.UtcNow, addedActivity.Location, userId, user.Name, user.ProfileImageUrl,
                addedActivity.DrinkType, addedActivity.DrinkId, addedActivity.DrinkName, addedActivity.AlcPrc, addedActivity.Volume);

            var savedActivity = await this.ActivityRepository.AddActivityAsync(activity);

            await AddToActivityAddedQueueAsync(savedActivity);
        }

        public async Task<List<ActivityAggregationDTO>> GetDrinkActivityAggregationAsync()
        {
            string userId = this.IdentityService.GetCurrentUserId();

            var startTime = DateTime.UtcNow.AddDays(-30).Date;
            
            var result = await this.ActivityRepository.GetActivitysForUser(userId, startTime, ActivityType.Drink);

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


        private async Task AddToActivityAddedQueueAsync(Activity savedActivity)
        {
            var queueClient = this.StorageAccessService.GetQueueReference(Constants.ActivityAddedQueueName);
            var message = new ActivityAddedMessage(savedActivity);
            await queueClient.AddMessageAsync(new Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage(JsonConvert.SerializeObject(message)));
        }

        public async Task<List<ActivityStatsDTO>> GetActivitiesAsync()
        {
            var activities = await this.ActivityRepository.GetActivitysAsync(new GetActivityFilterArgs(false));
            var userIds = activities.Select(a => a.UserId).Distinct();
            var userStats = await this.UserStatsRepository.GetStatisticsAsync(userIds);

            var result = activities.Select(a => new ActivityStatsDTO(a, userStats.First(u => u.UserId == a.UserId))).ToList();
            return result;
        }
    }
}
