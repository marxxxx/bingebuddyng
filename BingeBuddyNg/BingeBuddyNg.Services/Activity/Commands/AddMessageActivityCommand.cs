using BingeBuddyNg.Services.Activity.Domain;
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
        public AddMessageActivityCommand(string userId, string message, Location location, Venue.Venue venue)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Location = location;
            Venue = venue;
        }

        public string UserId { get; }
        public string Message { get; }
        public Location Location { get; }
        public Venue.Venue Venue { get; }
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

            var id = ActivityId.CreateNew(request.UserId, out var timestamp);
            var activity = Activity.CreateMessageActivity(id.Value, timestamp, request.Location, request.UserId, user.Name, request.Message, request.Venue);

            var savedActivity = await this.activityRepository.AddActivityAsync(activity.ToEntity());
            await activityRepository.AddToActivityAddedTopicAsync(savedActivity.Id);

            return savedActivity.Id;
        }
    }
}
