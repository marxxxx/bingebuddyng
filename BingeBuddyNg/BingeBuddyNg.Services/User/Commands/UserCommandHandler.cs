using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Infrastructure;
using MediatR;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Services.User.Commands
{
    public class UserCommandHandler :
        IRequestHandler<CreateOrUpdateUserCommand>,
        IRequestHandler<UpdateUserProfileImageCommand>,
        IRequestHandler<RemoveFriendCommand>,
        IRequestHandler<SetFriendMuteStateCommand>
    {
        private readonly IUserRepository userRepository;
        private readonly IActivityRepository activityRepository;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IStorageAccessService storageAccessService;

        public UserCommandHandler(IUserRepository userRepository, IActivityRepository activityRepository, IHttpClientFactory httpClientFactory, IStorageAccessService storageAccessService)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }
        
        public async Task<Unit> Handle(CreateOrUpdateUserCommand request, CancellationToken cancellationToken)
        {
            var result = await this.userRepository.CreateOrUpdateUserAsync(request);
            if (result.IsNewUser)
            {

                if (!string.IsNullOrEmpty(request.ProfileImageUrl))
                {
                    var httpClient = this.httpClientFactory.CreateClient();
                    using (var profileImageStream = await httpClient.GetStreamAsync(request.ProfileImageUrl))
                    {
                        await storageAccessService.SaveFileInBlobStorage(ContainerNames.ProfileImages, request.UserId, profileImageStream);
                    }
                }

                await activityRepository.AddActivityAsync(Activity.Activity.CreateRegistrationActivity(
                    User.BingeBuddyUserId, User.BingeBuddyUserName, new UserInfo(request.UserId, request.Name)));
            }


            if (result.NameHasChanged)
            {
                var activity = Activity.Activity.CreateRenameActivity(request.UserId, request.Name, result.OriginalUserName);
                await activityRepository.AddActivityAsync(activity);

                var renameMessage = new UserRenamedMessage(request.UserId, result.OriginalUserName, request.Name);
                await storageAccessService.AddQueueMessage(QueueNames.UserRenamed, renameMessage);
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(UpdateUserProfileImageCommand request, CancellationToken cancellationToken)
        {
            var user = await this.userRepository.FindUserAsync(request.UserId);
            using (var stream = request.Image.OpenReadStream())
            {
                await storageAccessService.SaveFileInBlobStorage(ContainerNames.ProfileImages, request.UserId, stream);
                var activity = Activity.Activity.CreateProfileImageUpdateActivity(request.UserId, user.Name);
                await activityRepository.AddActivityAsync(activity);
            }
            return Unit.Value;
        }

        public async Task<Unit> Handle(RemoveFriendCommand request, CancellationToken cancellationToken)
        {
            await this.userRepository.RemoveFriendAsync(request.UserId, request.FriendUserId);
            return Unit.Value;
        }

        public async Task<Unit> Handle(SetFriendMuteStateCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.FindUserAsync(request.UserId);
            user.SetFriendMuteState(request.FriendUserId, request.MuteState);

            var mutedUser = await userRepository.FindUserAsync(request.FriendUserId);
            mutedUser.SetMutedByFriendState(request.UserId, request.MuteState);

            Task.WaitAll(userRepository.UpdateUserAsync(user), userRepository.UpdateUserAsync(mutedUser));

            return Unit.Value;
        }
    }
}
