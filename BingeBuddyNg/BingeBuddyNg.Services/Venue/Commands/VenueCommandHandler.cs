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
        private readonly ITranslationService translationService;
        private readonly IActivityRepository activityRepository;

        public VenueCommandHandler(IUserRepository userRepository, IVenueUserRepository venueUserRepository, ITranslationService translationService, IActivityRepository activityRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.venueUserRepository = venueUserRepository ?? throw new ArgumentNullException(nameof(venueUserRepository));
            this.translationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        public async Task<Unit> Handle(EnterVenueCommand request, CancellationToken cancellationToken)
        {
            var user = await this.userRepository.FindUserAsync(request.UserId);
            if (user.CurrentVenue != request.Venue)
            {
                user.CurrentVenue = request.Venue;

                var message = await translationService.GetTranslationAsync(user.Language, "PersonalVenueEnterActivityMessage", request.Venue.Name);

                var tasks = new[]
                {
                    this.userRepository.UpdateUserAsync(user),
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
                user.CurrentVenue = null;

                var tasks = new[]
                {
                    this.userRepository.UpdateUserAsync(user),
                    this.venueUserRepository.RemoveUserFromVenueAsync(currentVenue.Id, request.UserId),
                    this.AddVenueActivityAsync(new AddVenueActivityDTO(user.Id, currentVenue, VenueAction.Leave))
                };

                await Task.WhenAll(tasks);
            }

            return Unit.Value;
        }

        private async Task AddVenueActivityAsync(AddVenueActivityDTO activity)
        {
            var user = await this.userRepository.FindUserAsync(activity.UserId);
            var timestamp = DateTime.UtcNow;
            var id = ActivityId.Create(timestamp, activity.UserId);
            var activityEntity = VenueActivity.Create(
                id.Value,
                timestamp,
                activity.UserId,
                user.Name,
                activity.Venue,
                activity.Action);

            var savedActivity = await this.activityRepository.AddActivityAsync(activityEntity);

            await activityRepository.AddToActivityAddedTopicAsync(savedActivity.Id);
        }
    }
}
