using BingeBuddyNg.Services.Activity;
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
        public IUserRepository UserRepository { get; }
        public IVenueUserRepository VenueUserRepository { get; }
        public ITranslationService TranslationService { get; }
        public IActivityRepository ActivityRepository { get; }

        public VenueCommandHandler(IUserRepository userRepository, IVenueUserRepository venueUserRepository, ITranslationService translationService, IActivityRepository activityRepository)
        {
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            VenueUserRepository = venueUserRepository ?? throw new ArgumentNullException(nameof(venueUserRepository));
            TranslationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
            ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        public async Task<Unit> Handle(EnterVenueCommand request, CancellationToken cancellationToken)
        {
            var user = await this.UserRepository.FindUserAsync(request.UserId);
            if (user.CurrentVenue != request.Venue)
            {
                user.CurrentVenue = request.Venue;

                var message = await TranslationService.GetTranslationAsync(user.Language, "PersonalVenueEnterActivityMessage", request.Venue.Name);

                var tasks = new[]
                {
                    this.UserRepository.UpdateUserAsync(user),
                    this.VenueUserRepository.AddUserToVenueAsync(request.Venue.Id, request.Venue.Name, request.UserId, user.Name),
                    this.AddVenueActivityAsync(new AddVenueActivityDTO(user.Id, message, request.Venue, VenueAction.Enter))
                };

                await Task.WhenAll(tasks);
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(LeaveVenueCommand request, CancellationToken cancellationToken)
        {
            var user = await this.UserRepository.FindUserAsync(request.UserId);
            var currentVenue = user.CurrentVenue;

            if (currentVenue != null)
            {
                user.CurrentVenue = null;
                var message = await TranslationService.GetTranslationAsync(user.Language, "PersonalVenueLeaveActivityMessage", currentVenue.Name);

                var tasks = new[]
                {
                    this.UserRepository.UpdateUserAsync(user),
                    this.VenueUserRepository.RemoveUserFromVenueAsync(currentVenue.Id, request.UserId),
                    this.AddVenueActivityAsync(new AddVenueActivityDTO(user.Id, message, currentVenue, VenueAction.Leave))
                };

                await Task.WhenAll(tasks);
            }

            return Unit.Value;
        }

        private async Task AddVenueActivityAsync(AddVenueActivityDTO activity)
        {
            var user = await this.UserRepository.FindUserAsync(activity.UserId);
            var activityEntity = Activity.Activity.CreateVenueActivity(DateTime.UtcNow, activity.UserId, user.Name,
                activity.Message, activity.Venue, activity.Action);

            var savedActivity = await this.ActivityRepository.AddActivityAsync(activityEntity);

            await ActivityRepository.AddToActivityAddedQueueAsync(savedActivity.Id);
        }

    }
}
