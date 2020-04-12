﻿using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Infrastructure;
using MediatR;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Services.User.Commands
{
    public class CreateOrUpdateUserCommand : IRequest
    {
        public CreateOrUpdateUserCommand(string userId, string name, string profileImageUrl, PushInfo pushInfo, string language)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ProfileImageUrl = profileImageUrl;
            PushInfo = pushInfo;
            Language = language;
        }

        public string UserId { get; }
        public string Name { get; }
        public string ProfileImageUrl { get; }
        public PushInfo PushInfo { get; }
        public string Language { get; }
    }

    public class CreateOrUpdateUserCommandHandler : IRequestHandler<CreateOrUpdateUserCommand>
    {
        private readonly IUserRepository userRepository;
        private readonly IActivityRepository activityRepository;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IStorageAccessService storageAccessService;

        public CreateOrUpdateUserCommandHandler(IUserRepository userRepository, IActivityRepository activityRepository, IHttpClientFactory httpClientFactory, IStorageAccessService storageAccessService)
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
    }
}
