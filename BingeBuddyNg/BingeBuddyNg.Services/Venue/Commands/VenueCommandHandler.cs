using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Activity.Domain;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Venue.Commands
{
    public class VenueCommandHandler :
        IRequestHandler<EnterVenueCommand>,
        IRequestHandler<LeaveVenueCommand>
    {
        private readonly IUserRepository userRepository;
        private readonly IVenueUserRepository venueUserRepository;
        private readonly IActivityRepository activityRepository;

        public VenueCommandHandler(IUserRepository userRepository, IVenueUserRepository venueUserRepository, IActivityRepository activityRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.venueUserRepository = venueUserRepository ?? throw new ArgumentNullException(nameof(venueUserRepository));
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        public async Task<Unit> Handle(EnterVenueCommand request, CancellationToken cancellationToken)
        {
            var user = await this.userRepository.FindUserAsync(request.UserId);

            if (user.CurrentVenue != request.Venue)
            {
                user.EnterVenue(request.Venue);

                var tasks = new[]
                {
                    this.userRepository.UpdateUserAsync(user.ToEntity()),
                    this.venueUserRepository.AddUserToVenueAsync(request.Venue.Id, request.Venue.Name, request.UserId, user.Name),
                    this.AddVenueActivityAsync(new AddVenueActivityDTO(user.Id, request.Venue, VenueAction.Enter))
                };

                await Task.WhenAll(tasks);
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(LeaveVenueCommand request, CancellationToken cancellationToken)
        {
            var user = await this.userRepository.FindUserAsync(request.UserId);
            var currentVenue = user.CurrentVenue;

            if (currentVenue != null)
            {
                user.LeaveVenue();

                var tasks = new[]
                {
                    this.userRepository.UpdateUserAsync(user.ToEntity()),
                    this.venueUserRepository.RemoveUserFromVenueAsync(currentVenue.Id, request.UserId),
                    this.AddVenueActivityAsync(new AddVenueActivityDTO(user.Id, currentVenue, VenueAction.Leave))
                };

                await Task.WhenAll(tasks);
            }

            return Unit.Value;
        }

        private async Task AddVenueActivityAsync(AddVenueActivityDTO venueActivity)
        {
            var user = await this.userRepository.FindUserAsync(venueActivity.UserId);
            var id = ActivityId.CreateNew(venueActivity.UserId, out var timestamp);
            var activity = Activity.Activity.CreateVenueActivity(
                id.Value,
                timestamp,
                venueActivity.UserId,
                user.Name,
                venueActivity.Venue,
                venueActivity.Action);

            var savedActivity = await this.activityRepository.AddActivityAsync(activity.ToEntity());

            await activityRepository.AddToActivityAddedTopicAsync(savedActivity.Id);
        }
    }
}
