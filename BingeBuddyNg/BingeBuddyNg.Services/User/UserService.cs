﻿using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Infrastructure;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.User
{
    public class UserService : IUserService
    {
        public IUserRepository UserRepository { get; }
        public IActivityRepository ActivityRepository { get; }
        public IStorageAccessService StorageAccessService { get; set; }
        public ITranslationService TranslationService { get; }

        public UserService(IUserRepository userRepository,
            IActivityRepository activityRepository,
            IStorageAccessService storageAccessService,
            ITranslationService translationService)
        {
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.StorageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
            this.TranslationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
        }

        public async Task<UpdateUserResponseDTO> UpdateUserProfileAsync(User user)
        {
            var result = await this.UserRepository.CreateOrUpdateUserAsync(user);
            if (result.IsNewUser)
            {
                await ActivityRepository.AddActivityAsync(Activity.Activity.CreateRegistrationActivity(
                    Services.User.User.BingeBuddyUserId, Services.User.User.BingeBuddyUserName, user.ToUserInfo()));
            }

            // enqueue profile image update change
            if (result.ProfilePicHasChanged)
            {
                var queue = StorageAccessService.GetQueueReference(Shared.Constants.QueueNames.ProfileUpdate);
                var message = new ProfileUpdateMessage(user.Id, user.ProfileImageUrl);
                await queue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(message)));
            }

            if (result.NameHasChanged)
            {
                var activity = Activity.Activity.CreateRenameActivity(user.Id, user.Name, result.OriginalUserName);
                await ActivityRepository.AddActivityAsync(activity);
            }

            var response = new UpdateUserResponseDTO(!user.Weight.HasValue, user.Gender == Gender.Unknown);
            return response;
        }
    }
}
