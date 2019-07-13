using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Infrastructure;
using MediatR;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
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
        public UserCommandHandler(IUserRepository userRepository, IActivityRepository activityRepository, IHttpClientFactory httpClientFactory, IStorageAccessService storageAccessService)
        {
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            StorageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public IUserRepository UserRepository { get; }
        public IActivityRepository ActivityRepository { get; }
        public IHttpClientFactory HttpClientFactory { get; }
        public IStorageAccessService StorageAccessService { get; }

      
        public async Task<Unit> Handle(CreateOrUpdateUserCommand request, CancellationToken cancellationToken)
        {
            var result = await this.UserRepository.CreateOrUpdateUserAsync(request);
            if (result.IsNewUser)
            {

                if (!string.IsNullOrEmpty(request.ProfileImageUrl))
                {
                    var httpClient = this.HttpClientFactory.CreateClient();
                    using (var profileImageStream = await httpClient.GetStreamAsync(request.ProfileImageUrl))
                    {
                        await StorageAccessService.SaveFileInBlobStorage(ContainerNames.ProfileImages, request.UserId, profileImageStream);
                    }
                }

                await ActivityRepository.AddActivityAsync(Activity.Activity.CreateRegistrationActivity(
                    Services.User.User.BingeBuddyUserId, Services.User.User.BingeBuddyUserName, new UserInfo(request.UserId, request.Name)));
            }


            if (result.NameHasChanged)
            {
                var activity = Activity.Activity.CreateRenameActivity(request.UserId, request.Name, result.OriginalUserName);
                await ActivityRepository.AddActivityAsync(activity);

                var renameMessage = new UserRenamedMessage(request.UserId, result.OriginalUserName, request.Name);
                await StorageAccessService.AddQueueMessage(QueueNames.UserRenamed, renameMessage);
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(UpdateUserProfileImageCommand request, CancellationToken cancellationToken)
        {
            var user = await this.UserRepository.FindUserAsync(request.UserId);
            using (var stream = request.Image.OpenReadStream())
            {
                await StorageAccessService.SaveFileInBlobStorage(ContainerNames.ProfileImages, request.UserId, stream);
                var activity = Activity.Activity.CreateProfileImageUpdateActivity(request.UserId, user.Name);
                await ActivityRepository.AddActivityAsync(activity);
            }
            return Unit.Value;
        }

        public async Task<Unit> Handle(RemoveFriendCommand request, CancellationToken cancellationToken)
        {
            await this.UserRepository.RemoveFriendAsync(request.UserId, request.FriendUserId);
            return Unit.Value;
        }

        public async Task<Unit> Handle(SetFriendMuteStateCommand request, CancellationToken cancellationToken)
        {
            var user = await UserRepository.FindUserAsync(request.UserId);
            user.SetFriendMuteState(request.FriendUserId, request.MuteState);

            var mutedUser = await UserRepository.FindUserAsync(request.FriendUserId);
            mutedUser.SetMutedByFriendState(request.UserId, request.MuteState);

            Task.WaitAll(UserRepository.UpdateUserAsync(user), UserRepository.UpdateUserAsync(mutedUser));

            return Unit.Value;
        }
    }
}
