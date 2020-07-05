using System;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Core.Venue;
using BingeBuddyNg.Core.Venue.DTO;
using MediatR;

namespace BingeBuddyNg.Core.Activity.Commands
{
    public class AddMessageActivityCommand :  IRequest<string>
    {
        public AddMessageActivityCommand(string userId, string message, Location location, VenueDTO venue)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Location = location;
            Venue = venue;
        }

        public string UserId { get; }
        public string Message { get; }
        public Location Location { get; }
        public VenueDTO Venue { get; }
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

            var activity = Domain.Activity.CreateMessageActivity(request.Location, request.UserId, user.Name, request.Message, request.Venue?.ToDomain());

            var savedActivity = await this.activityRepository.AddActivityAsync(activity.ToEntity());
            await activityRepository.AddToActivityAddedTopicAsync(savedActivity.Id);

            return savedActivity.Id;
        }
    }
}
