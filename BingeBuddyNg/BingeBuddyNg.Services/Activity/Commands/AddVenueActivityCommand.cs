using BingeBuddyNg.Services.Activity.Domain;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.Venue;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Activity.Commands
{
    public class AddVenueActivityCommand : IRequest<string>
    {
        public AddVenueActivityCommand(string userId, string message, VenueAction action, Venue.Venue venue)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Action = action;
            Venue = venue ?? throw new ArgumentNullException(nameof(venue));
        }

        public string UserId { get; }
        public string Message { get; }
        public VenueAction Action { get; }
        public Venue.Venue Venue { get; }
    }

    public class AddVenueActivityCommandHandler : IRequestHandler<AddVenueActivityCommand, string>
    {
        private readonly IUserRepository userRepository;
        private readonly IActivityRepository activityRepository;

        public AddVenueActivityCommandHandler(
            IUserRepository userRepository,
            IActivityRepository activityRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        public async Task<string> Handle(AddVenueActivityCommand request, CancellationToken cancellationToken)
        {
            var user = await this.userRepository.GetUserAsync(request.UserId);

            var activity = Activity.CreateVenueActivity(
                request.UserId,
                user.Name,
                request.Venue,
                request.Action);

            var savedActivity = await this.activityRepository.AddActivityAsync(activity.ToEntity());

            await activityRepository.AddToActivityAddedTopicAsync(savedActivity.Id);

            return savedActivity.Id;
        }
    }
}
