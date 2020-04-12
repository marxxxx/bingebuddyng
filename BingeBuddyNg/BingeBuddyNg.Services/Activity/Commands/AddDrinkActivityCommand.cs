﻿using BingeBuddyNg.Services.Drink;
using BingeBuddyNg.Services.Infrastructure.Messaging;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.Venue;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Activity.Commands
{
    public class AddDrinkActivityCommand : IRequest<string>
    {
        public AddDrinkActivityCommand(string userId, string drinkId, DrinkType drinkType, string drinkName, double alcPrc, double volume, Location location, VenueModel venue)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            DrinkId = drinkId ?? throw new ArgumentNullException(nameof(drinkId));
            DrinkType = drinkType;
            DrinkName = drinkName ?? throw new ArgumentNullException(nameof(drinkName));
            AlcPrc = alcPrc;
            Volume = volume;
            Location = location;
            Venue = venue;
        }

        public string UserId { get; }
        public string DrinkId { get; }
        public DrinkType DrinkType { get; }
        public string DrinkName { get; }
        public double AlcPrc { get; }
        public double Volume { get; }
        public Location Location { get; }
        public VenueModel Venue { get; }
    }

    public class AddDrinkActivityCommandHandler :
            IRequestHandler<AddDrinkActivityCommand, string>
    {
        private readonly IUserRepository userRepository;
        private readonly IActivityRepository activityRepository;
        private readonly IMessagingService messagingService;

        public AddDrinkActivityCommandHandler(
            IUserRepository userRepository,
            IActivityRepository activityRepository,
            IMessagingService messagingService)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
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
    }
}
