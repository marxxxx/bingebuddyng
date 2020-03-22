﻿using BingeBuddyNg.Services.Drink;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Infrastructure.Messaging;
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
            IRequestHandler<AddDrinkActivityCommand, string>,
            IRequestHandler<AddImageActivityCommand, string>,
            IRequestHandler<AddMessageActivityCommand, string>,
            IRequestHandler<AddVenueActivityCommand, string>,
            IRequestHandler<AddReactionCommand>,
            IRequestHandler<DeleteActivityCommand>
    {
        private readonly IUserRepository userRepository;
        private readonly IActivityRepository activityRepository;
        private readonly IStorageAccessService storageAccessService;
        private readonly IMessagingService messagingService;

        public ActivityCommandHandler(
            IUserRepository userRepository, 
            IActivityRepository activityRepository, 
            IStorageAccessService storageAccessService,
            IMessagingService messagingService)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
            this.messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
        }

        public async Task<string> Handle(AddImageActivityCommand request, CancellationToken cancellationToken)
        {
            var user = await this.userRepository.FindUserAsync(request.UserId);

            // store file in blob storage
            string imageUrlOriginal = await storageAccessService.SaveFileInBlobStorage("img", "activities", request.FileName, request.Stream);

            var activity = Activity.CreateImageActivity(DateTime.UtcNow, request.Location, request.UserId, user.Name, imageUrlOriginal);

            var savedActivity = await this.activityRepository.AddActivityAsync(activity);

            await activityRepository.AddToActivityAddedQueueAsync(savedActivity.Id);

            return savedActivity.Id;
        }

        public async Task<string> Handle(AddMessageActivityCommand request, CancellationToken cancellationToken)
        {
            var user = await this.userRepository.FindUserAsync(request.UserId);

            var activity = Activity.CreateMessageActivity(DateTime.UtcNow, request.Location, request.UserId, user.Name, request.Message);
            activity.Venue = request.Venue;

            var savedActivity = await this.activityRepository.AddActivityAsync(activity);
            await activityRepository.AddToActivityAddedQueueAsync(savedActivity.Id);

            return savedActivity.Id;
        }

        public async Task<string> Handle(AddVenueActivityCommand request, CancellationToken cancellationToken)
        {
            var user = await this.userRepository.FindUserAsync(request.UserId);
            var activityEntity = Activity.CreateVenueActivity(DateTime.UtcNow, request.UserId, user.Name,
                request.Message, request.Venue, request.Action);

            var savedActivity = await this.activityRepository.AddActivityAsync(activityEntity);

            await activityRepository.AddToActivityAddedQueueAsync(savedActivity.Id);

            return savedActivity.Id;
        }

        public async Task<Unit> Handle(AddReactionCommand request, CancellationToken cancellationToken)
        {
            var activity = await this.activityRepository.GetActivityAsync(request.ActivityId);

            var reactingUser = await this.userRepository.FindUserAsync(request.UserId);

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

            await this.activityRepository.UpdateActivityAsync(activity);

            // add to queue
            var queueClient = this.storageAccessService.GetQueueReference(Constants.QueueNames.ReactionAdded);
            var message = new ReactionAddedMessage(request.ActivityId, request.Type, request.UserId, request.Comment);
            await queueClient.AddMessageAsync(new Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage(JsonConvert.SerializeObject(message)));

            return Unit.Value;
        }

        public async Task<string> Handle(AddDrinkActivityCommand request, CancellationToken cancellationToken)
        {
            var user = await this.userRepository.FindUserAsync(request.UserId);

            var activity = Activity.CreateDrinkActivity(DateTime.UtcNow, request.Location, request.UserId, user.Name,
                request.DrinkType, request.DrinkId, request.DrinkName, request.AlcPrc, request.Volume);
            activity.Venue = request.Venue;

            var savedActivity = await this.activityRepository.AddActivityAsync(activity);

            await activityRepository.AddToActivityAddedQueueAsync(savedActivity.Id);

            await messagingService.SendMessageAsync(new DrinkEventMessage(request.UserId, request.DrinkId, activity.Timestamp));

            return savedActivity.Id;
        }

        public async Task<Unit> Handle(DeleteActivityCommand request, CancellationToken cancellationToken)
        {
            await activityRepository.DeleteActivityAsync(request.UserId, request.ActivityId);
            return Unit.Value;
        }
    }
}
