using BingeBuddyNg.Services.DTO;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Messages;
using BingeBuddyNg.Services.Models;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
            var user = await this.UserRepository.FindUserAsync(userId);

            var activity = Activity.CreateMessageActivity(DateTime.UtcNow, addedActivity.Location, userId, user.Name, user.ProfileImageUrl, addedActivity.Message);

            var savedActivity = await this.ActivityRepository.AddActivityAsync(activity);
            await AddToActivityAddedQueueAsync(savedActivity);
        }

        public async Task AddDrinkActivityAsync(AddDrinkActivityDTO addedActivity)
        {
            var userId = this.IdentityService.GetCurrentUserId();
            var user = await this.UserRepository.FindUserAsync(userId);

            var activity = Activity.CreateDrinkActivity(DateTime.UtcNow, addedActivity.Location, userId, user.Name, user.ProfileImageUrl,
                addedActivity.DrinkType, addedActivity.DrinkId, addedActivity.DrinkName, addedActivity.AlcPrc, addedActivity.Volume);

            var savedActivity = await this.ActivityRepository.AddActivityAsync(activity);

            await AddToActivityAddedQueueAsync(savedActivity);
        }

        public async Task AddImageActivityAsync(Stream stream, string fileName, Location location)
        {
            var userId = this.IdentityService.GetCurrentUserId();
            var user = await this.UserRepository.FindUserAsync(userId);

            // store file in blob storage
            string imageUrlOriginal = await StorageAccessService.SaveFileInBlobStorage("img", "activities", fileName, stream);
            
            var activity = Activity.CreateImageActivity(DateTime.UtcNow, location, userId, user.Name, user.ProfileImageUrl, imageUrlOriginal);

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
                .Select(t => new ActivityAggregationDTO()
                {
                    Count = t.Count(),
                    CountBeer = t.Count(d=>d.DrinkType == DrinkType.Beer),
                    CountWine = t.Count(d => d.DrinkType == DrinkType.Wine),
                    CountShots = t.Count(d => d.DrinkType == DrinkType.Shot),
                    CountAnti = t.Count(d => d.DrinkType == DrinkType.Anti),
                    CountAlc = t.Count(d => d.DrinkType != DrinkType.Anti),
                    Day = t.Key
                })
                .ToList();

            // now fill holes of last 30 days
            for(int i=-30; i<0; i++)
            {
                var date = DateTime.UtcNow.AddDays(i).Date;
                var hasData = groupedByDay.Any(d => d.Day == date);
                if (hasData == false)
                {
                    groupedByDay.Add(new ActivityAggregationDTO(date));
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

       

        public async Task<PagedQueryResult<ActivityStatsDTO>> GetActivityFeedAsync(string userId, TableContinuationToken continuationToken = null)
        {
            var callingUser = await this.UserRepository.FindUserAsync(userId);
            var visibleUserIds = callingUser.Friends.Select(f => f.UserId).Except(callingUser.MutedByFriendUserIds);
            

            // TODO: Use Constant for Page Size
            var activities = await this.ActivityRepository.GetActivityFeedAsync(new GetActivityFilterArgs(false, visibleUserIds, 20, continuationToken));
            var userIds = activities.ResultPage.Select(a => a.UserId).Distinct();
            var userStats = await this.UserStatsRepository.GetStatisticsAsync(userIds);

            var result = activities.ResultPage.Select(a => new ActivityStatsDTO(a, userStats.First(u => u.UserId == a.UserId))).ToList();
            return new PagedQueryResult<ActivityStatsDTO>(result, activities.ContinuationToken);
        }

        public async Task AddReactionAsync(ReactionDTO reaction)
        {
            var userId = this.IdentityService.GetCurrentUserId();
            
            // add to queue
            var queueClient = this.StorageAccessService.GetQueueReference(Constants.ReactionAddedQueueName);
            var message = new ReactionAddedMessage(reaction.ActivityId, reaction.Type, userId, reaction.Comment);
            await queueClient.AddMessageAsync(new Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage(JsonConvert.SerializeObject(message)));
        }

    }
}
