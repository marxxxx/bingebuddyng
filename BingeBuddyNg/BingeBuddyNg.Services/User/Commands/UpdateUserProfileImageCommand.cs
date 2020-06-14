using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Activity.Domain;
using BingeBuddyNg.Services.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Services.User.Commands
{
    public class UpdateUserProfileImageCommand : IRequest
    {
        public UpdateUserProfileImageCommand(string userId, IFormFile image)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Image = image ?? throw new ArgumentNullException(nameof(image));
        }

        public string UserId { get; }
        public IFormFile Image { get; }
    }

    public class UpdateUserProfileImageCommandHandler : IRequestHandler<UpdateUserProfileImageCommand>
    {
        private readonly IUserRepository userRepository;
        private readonly IActivityRepository activityRepository;
        private readonly IStorageAccessService storageAccessService;

        public UpdateUserProfileImageCommandHandler(IUserRepository userRepository, IActivityRepository activityRepository, IStorageAccessService storageAccessService)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public async Task<Unit> Handle(UpdateUserProfileImageCommand request, CancellationToken cancellationToken)
        {
            var user = await this.userRepository.FindUserAsync(request.UserId);
            using (var stream = request.Image.OpenReadStream())
            {
                await storageAccessService.SaveFileInBlobStorage(ContainerNames.ProfileImages, request.UserId, stream);

                var timestamp = DateTime.UtcNow;
                var id = ActivityId.Create(timestamp, request.UserId);

                var activity = Activity.Activity.CreateProfileImageUpdateActivity(id.Value, timestamp, request.UserId, user.Name);
                await activityRepository.AddActivityAsync(activity.ToEntity());
            }
            return Unit.Value;
        }
    }
}
