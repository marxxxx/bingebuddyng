using BingeBuddyNg.Services.Drink;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Shared;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Activity.Commands
{
    public class ActivityCommandHandler :   
            IRequestHandler<AddDrinkActivityCommand>,
            IRequestHandler<AddImageActivityCommand>,
            IRequestHandler<AddMessageActivityCommand>,
            IRequestHandler<AddVenueActivityCommand>,
            IRequestHandler<AddReactionCommand>,
            IRequestHandler<DeleteActivityCommand>
    {
        public IUserRepository UserRepository { get; }
        public IActivityRepository ActivityRepository { get; }
        public IStorageAccessService StorageAccessService { get; }

        public ActivityCommandHandler(IUserRepository userRepository, IActivityRepository activityRepository, IStorageAccessService storageAccessService)
        {
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            StorageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public async Task<Unit> Handle(AddImageActivityCommand request, CancellationToken cancellationToken)
        {
            var user = await this.UserRepository.FindUserAsync(request.UserId);

            // store file in blob storage
            string imageUrlOriginal = await StorageAccessService.SaveFileInBlobStorage("img", "activities", request.FileName, request.Stream);

            var activity = Activity.CreateImageActivity(DateTime.UtcNow, request.Location, request.UserId, user.Name, imageUrlOriginal);

            var savedActivity = await this.ActivityRepository.AddActivityAsync(activity);

            await ActivityRepository.AddToActivityAddedQueueAsync(savedActivity.Id);

            return Unit.Value;
        }

        public async Task<Unit> Handle(AddMessageActivityCommand request, CancellationToken cancellationToken)
        {
            var user = await this.UserRepository.FindUserAsync(request.UserId);

            var activity = Activity.CreateMessageActivity(DateTime.UtcNow, request.Location, request.UserId, user.Name, request.Message);
            activity.Venue = request.Venue;

            var savedActivity = await this.ActivityRepository.AddActivityAsync(activity);
            await ActivityRepository.AddToActivityAddedQueueAsync(savedActivity.Id);

            return Unit.Value;
        }

        public async Task<Unit> Handle(AddVenueActivityCommand request, CancellationToken cancellationToken)
        {
            var user = await this.UserRepository.FindUserAsync(request.UserId);
            var activityEntity = Activity.CreateVenueActivity(DateTime.UtcNow, request.UserId, user.Name,
                request.Message, request.Venue, request.Action);

            var savedActivity = await this.ActivityRepository.AddActivityAsync(activityEntity);

            await ActivityRepository.AddToActivityAddedQueueAsync(savedActivity.Id);

            return Unit.Value;
        }

        public async Task<Unit> Handle(AddReactionCommand request, CancellationToken cancellationToken)
        {
            var activity = await this.ActivityRepository.GetActivityAsync(request.ActivityId);

            var reactingUser = await this.UserRepository.FindUserAsync(request.UserId);

            switch (request.Type)
            {

                case ReactionType.Cheers:
                    activity.AddCheers(new Reaction(request.UserId, reactingUser.Name));
                    break;
                case ReactionType.Like:
                    activity.AddLike(new Reaction(request.UserId, reactingUser.Name));
                    break;
                case ReactionType.Comment:
                    activity.AddComment(new CommentReaction(request.UserId, reactingUser.Name, request.Comment));
                    break;
            }

            await this.ActivityRepository.UpdateActivityAsync(activity);

            

            // add to queue
            var queueClient = this.StorageAccessService.GetQueueReference(Constants.QueueNames.ReactionAdded);
            var message = new ReactionAddedMessage(request.ActivityId, request.Type, request.UserId, request.Comment);
            await queueClient.AddMessageAsync(new Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage(JsonConvert.SerializeObject(message)));

            return Unit.Value;
        }

        public async Task<Unit> Handle(AddDrinkActivityCommand request, CancellationToken cancellationToken)
        {
            var user = await this.UserRepository.FindUserAsync(request.UserId);

            int drinkCount = 0;
            if (request.DrinkType != DrinkType.Anti)
            {
                // immediately update drink count
                var drinkActivitys = await ActivityRepository.GetActivitysForUserAsync(request.UserId, DateTime.UtcNow.Subtract(TimeSpan.FromHours(12)), ActivityType.Drink);
                drinkCount = drinkActivitys.Where(a => a.DrinkType != DrinkType.Anti).Count() + 1;
            }

            var activity = Activity.CreateDrinkActivity(DateTime.UtcNow, request.Location, request.UserId, user.Name,
                request.DrinkType, request.DrinkId, request.DrinkName, request.AlcPrc, request.Volume);
            activity.Venue = request.Venue;
            activity.DrinkCount = drinkCount;

            var savedActivity = await this.ActivityRepository.AddActivityAsync(activity);

            await ActivityRepository.AddToActivityAddedQueueAsync(savedActivity.Id);

            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteActivityCommand request, CancellationToken cancellationToken)
        {
            await ActivityRepository.DeleteActivityAsync(request.UserId, request.ActivityId);
            return Unit.Value;
        }

    }
}
