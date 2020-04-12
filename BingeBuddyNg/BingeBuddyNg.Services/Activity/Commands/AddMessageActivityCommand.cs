using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.Venue;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Activity.Commands
{
    public class AddMessageActivityCommand :  IRequest<string>
    {
        public AddMessageActivityCommand(string userId, string message, Location location, VenueModel venue)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Location = location;
            Venue = venue;
        }

        public string UserId { get; }
        public string Message { get; }
        public Location Location { get; }
        public VenueModel Venue { get; }
    }

    public class AddMessageActivityCommandHandler :
           IRequestHandler<AddMessageActivityCommand, string>
    {
        private readonly IUserRepository userRepository;
        private readonly IActivityRepository activityRepository;

        public AddMessageActivityCommandHandler(
            IUserRepository userRepository,
            IActivityRepository activityRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
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
    }
}
