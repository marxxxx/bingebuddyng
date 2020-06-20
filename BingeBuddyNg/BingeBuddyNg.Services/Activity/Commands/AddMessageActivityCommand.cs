using System;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Services.User;
using MediatR;

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
            var user = await this.userRepository.GetUserAsync(request.UserId);

            var activity = Activity.CreateMessageActivity(request.Location, request.UserId, user.Name, request.Message, request.Venue);

            var savedActivity = await this.activityRepository.AddActivityAsync(activity.ToEntity());
            await activityRepository.AddToActivityAddedTopicAsync(savedActivity.Id);

            return savedActivity.Id;
        }
    }
}
