using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Drink;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace BingeBuddyNg.Services.Activity
{
    public class ActivityService : IActivityService
    {
        private const int MaxQueryLoopCount = 5;
        public IIdentityService IdentityService { get; }
        public IUserRepository UserRepository { get; }
        public IActivityRepository ActivityRepository { get; }
        public IUserStatsRepository UserStatsRepository { get; }
        public StorageAccessService StorageAccessService { get; }

        private ILogger<ActivityService> logger;

        public ActivityService(IIdentityService identityService,
            IUserRepository userRepository,
            IActivityRepository activityRepository,
            IUserStatsRepository userStatsRepository,
            StorageAccessService storageAccessService,
            ILogger<ActivityService> logger)
        {
            this.IdentityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.UserStatsRepository = userStatsRepository ?? throw new ArgumentNullException(nameof(userStatsRepository));
            this.StorageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task AddMessageActivityAsync(AddMessageActivityDTO addedActivity)
        {
            var userId = this.IdentityService.GetCurrentUserId();
            var user = await this.UserRepository.FindUserAsync(userId);

            var activity = Activity.CreateMessageActivity(DateTime.UtcNow, addedActivity.Location, userId, user.Name, addedActivity.Message);
            activity.Venue = addedActivity.Venue;

            var savedActivity = await this.ActivityRepository.AddActivityAsync(activity);
            await AddToActivityAddedQueueAsync(savedActivity.Id);
        }



        public async Task AddDrinkActivityAsync(AddDrinkActivityDTO addedActivity)
        {
            var userId = this.IdentityService.GetCurrentUserId();
            var user = await this.UserRepository.FindUserAsync(userId);

            int drinkCount = 0;
            if(addedActivity.DrinkType != DrinkType.Anti)
            {
                // immediately update drink count
                var drinkActivitys = await ActivityRepository.GetActivitysForUserAsync(userId, DateTime.UtcNow.Subtract(TimeSpan.FromHours(12)), ActivityType.Drink);
                drinkCount = drinkActivitys.Where(a=>a.DrinkType != DrinkType.Anti).Count() + 1;
            }

            var activity = Activity.CreateDrinkActivity(DateTime.UtcNow, addedActivity.Location, userId, user.Name, 
                addedActivity.DrinkType, addedActivity.DrinkId, addedActivity.DrinkName, addedActivity.AlcPrc, addedActivity.Volume);
            activity.Venue = addedActivity.Venue;
            activity.DrinkCount = drinkCount;

            var savedActivity = await this.ActivityRepository.AddActivityAsync(activity);

            await AddToActivityAddedQueueAsync(savedActivity.Id);
        }

        public async Task AddImageActivityAsync(Stream stream, string fileName, Location location)
        {
            var userId = this.IdentityService.GetCurrentUserId();
            var user = await this.UserRepository.FindUserAsync(userId);

            // store file in blob storage
            string imageUrlOriginal = await StorageAccessService.SaveFileInBlobStorage("img", "activities", fileName, stream);

            var activity = Activity.CreateImageActivity(DateTime.UtcNow, location, userId, user.Name, imageUrlOriginal);

            var savedActivity = await this.ActivityRepository.AddActivityAsync(activity);

            await AddToActivityAddedQueueAsync(savedActivity.Id);
        }

        public async Task AddVenueActivityAsync(AddVenueActivityDTO activity)
        {
            var user = await this.UserRepository.FindUserAsync(activity.UserId);
            var activityEntity = Activity.CreateVenueActivity(DateTime.UtcNow, activity.UserId, user.Name, 
                activity.Message, activity.Venue, activity.Action);

            var savedActivity = await this.ActivityRepository.AddActivityAsync(activityEntity);

            await AddToActivityAddedQueueAsync(savedActivity.Id);
        }

        public async Task<List<ActivityAggregationDTO>> GetDrinkActivityAggregationAsync()
        {
            string userId = this.IdentityService.GetCurrentUserId();

            var startTime = DateTime.UtcNow.AddDays(-30).Date;

            var result = await this.ActivityRepository.GetActivitysForUserAsync(userId, startTime, ActivityType.Drink);

            var groupedByDay = result.GroupBy(t => t.Timestamp.Date)
                .OrderBy(t => t.Key)
                .Select(t => new ActivityAggregationDTO()
                {
                    Count = t.Count(),
                    CountBeer = t.Count(d => d.DrinkType == DrinkType.Beer),
                    CountWine = t.Count(d => d.DrinkType == DrinkType.Wine),
                    CountShots = t.Count(d => d.DrinkType == DrinkType.Shot),
                    CountAnti = t.Count(d => d.DrinkType == DrinkType.Anti),
                    CountAlc = t.Count(d => d.DrinkType != DrinkType.Anti),
                    Day = t.Key
                })
                .ToList();

            // now fill holes of last 30 days
            for (int i = -30; i < 0; i++)
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

        private async Task AddToActivityAddedQueueAsync(string activityId)
        {
            var queueClient = this.StorageAccessService.GetQueueReference(Constants.QueueNames.ActivityAdded);
            var message = new ActivityAddedMessage(activityId);
            await queueClient.AddMessageAsync(new Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage(JsonConvert.SerializeObject(message)));
        }



        public async Task<PagedQueryResult<ActivityStatsDTO>> GetActivityFeedAsync(string userId, TableContinuationToken continuationToken = null)
        {
            var callingUser = await this.UserRepository.FindUserAsync(userId);

            // TODO: Use Constant for Page Size
            var visibleUserIds = callingUser.GetVisibleFriendUserIds();
            var activities = await this.ActivityRepository.GetActivityFeedAsync(new GetActivityFilterArgs(visibleUserIds, continuationToken));

            var userIds = activities.ResultPage.Select(a => a.UserId).Distinct();
            var userStats = await this.UserStatsRepository.GetStatisticsAsync(userIds);

            var result = activities.ResultPage.Select(a => new ActivityStatsDTO(a, userStats.First(u => u.UserId == a.UserId))).ToList();
            return new PagedQueryResult<ActivityStatsDTO>(result, activities.ContinuationToken);
        }

     


        public async Task AddReactionAsync(ReactionDTO reaction)
        {
            var userId = this.IdentityService.GetCurrentUserId();
            var activity = await this.ActivityRepository.GetActivityAsync(reaction.ActivityId);

            var reactingUser = await this.UserRepository.FindUserAsync(userId);

            switch (reaction.Type)
            {

                case ReactionType.Cheers:
                    activity.AddCheers(new Reaction(userId, reactingUser.Name, reactingUser.ProfileImageUrl));
                    break;
                case ReactionType.Like:
                    activity.AddLike(new Reaction(userId, reactingUser.Name, reactingUser.ProfileImageUrl));
                    break;
                case ReactionType.Comment:
                    activity.AddComment(new CommentReaction(userId, reactingUser.Name, reactingUser.ProfileImageUrl, reaction.Comment));
                    break;
            }

            await this.ActivityRepository.UpdateActivityAsync(activity);
            

            // add to queue
            var queueClient = this.StorageAccessService.GetQueueReference(Constants.QueueNames.ReactionAdded);
            var message = new ReactionAddedMessage(reaction.ActivityId, reaction.Type, userId, reaction.Comment);
            await queueClient.AddMessageAsync(new Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage(JsonConvert.SerializeObject(message)));
        }

    }
}
