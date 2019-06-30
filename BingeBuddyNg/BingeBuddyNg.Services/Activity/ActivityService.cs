using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Shared;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Activity
{
    public class ActivityService : IActivityService
    {
        public IUserRepository UserRepository { get; }
        public IActivityRepository ActivityRepository { get; }
        public StorageAccessService StorageAccessService { get; }

        private ILogger<ActivityService> logger;

        public ActivityService(IUserRepository userRepository,
            IActivityRepository activityRepository,
            StorageAccessService storageAccessService,
            ILogger<ActivityService> logger)
        {
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.StorageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddVenueActivityAsync(AddVenueActivityDTO activity)
        {
            var user = await this.UserRepository.FindUserAsync(activity.UserId);
            var activityEntity = Activity.CreateVenueActivity(DateTime.UtcNow, activity.UserId, user.Name, 
                activity.Message, activity.Venue, activity.Action);

            var savedActivity = await this.ActivityRepository.AddActivityAsync(activityEntity);

            await AddToActivityAddedQueueAsync(savedActivity.Id);
        }

        private async Task AddToActivityAddedQueueAsync(string activityId)
        {
            var queueClient = this.StorageAccessService.GetQueueReference(Constants.QueueNames.ActivityAdded);
            var message = new ActivityAddedMessage(activityId);
            await queueClient.AddMessageAsync(new Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage(JsonConvert.SerializeObject(message)));
        }


    }
}
