using System;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.Drink;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Core.Venue;
using BingeBuddyNg.Core.Venue.DTO;
using BingeBuddyNg.Services.Activity;
using MediatR;

namespace BingeBuddyNg.Core.Activity.Commands
{
    public class AddDrinkActivityCommand : IRequest<string>
    {
        public AddDrinkActivityCommand(string userId, string drinkId, DrinkType drinkType, string drinkName, double alcPrc, double volume, Location location, VenueDTO venue)
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
        public VenueDTO Venue { get; }
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
            var user = await this.userRepository.GetUserAsync(request.UserId);

            var activity = Domain.Activity.CreateDrinkActivity(request.Location, request.UserId, user.Name,
                request.DrinkType, request.DrinkId, request.DrinkName, request.AlcPrc, request.Volume, request.Venue?.ToDomain());

            var savedActivity = await this.activityRepository.AddActivityAsync(activity.ToEntity());

            await activityRepository.AddToActivityAddedTopicAsync(savedActivity.Id);

            await messagingService.SendMessageAsync(new DrinkEventMessage(request.UserId, request.DrinkId, activity.Timestamp));

            return savedActivity.Id;
        }
    }
}
