using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Services.User
{
    public class UserService : IUserService
    {
        public IUserRepository UserRepository { get; }
        public IActivityRepository ActivityRepository { get; }
        public IStorageAccessService StorageAccessService { get; set; }
        public ITranslationService TranslationService { get; }
        public IHttpClientFactory HttpClientFactory { get; }

        public UserService(IUserRepository userRepository,
            IActivityRepository activityRepository,
            IStorageAccessService storageAccessService,
            ITranslationService translationService,
            IHttpClientFactory httpClientFactory)
        {
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.StorageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
            this.TranslationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
            this.HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<UpdateUserResponseDTO> UpdateUserProfileAsync(User user)
        {
            var result = await this.UserRepository.CreateOrUpdateUserAsync(user);
            if (result.IsNewUser)
            {

                if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                {
                    var httpClient = this.HttpClientFactory.CreateClient();
                    using (var profileImageStream = await httpClient.GetStreamAsync(user.ProfileImageUrl))
                    {
                        await StorageAccessService.SaveFileInBlobStorage(ContainerNames.ProfileImages, user.Id, profileImageStream);
                    }
                }

                await ActivityRepository.AddActivityAsync(Activity.Activity.CreateRegistrationActivity(
                    Services.User.User.BingeBuddyUserId, Services.User.User.BingeBuddyUserName, user.ToUserInfo()));

            }


            if (result.NameHasChanged)
            {
                var activity = Activity.Activity.CreateRenameActivity(user.Id, user.Name, result.OriginalUserName);
                await ActivityRepository.AddActivityAsync(activity);

                var renameMessage = new UserRenamedMessage(user.Id, result.OriginalUserName, user.Name);
                await StorageAccessService.AddQueueMessage(QueueNames.UserRenamed, renameMessage);
            }

            var response = new UpdateUserResponseDTO(!user.Weight.HasValue, user.Gender == Gender.Unknown);
            return response;
        }

        public async Task UpdateUserProfilePicAsync(string userId, IFormFile file)
        {
            var user = await this.UserRepository.FindUserAsync(userId);
            using (var stream = file.OpenReadStream())
            {
                await StorageAccessService.SaveFileInBlobStorage(ContainerNames.ProfileImages, userId, stream);
                var activity = Activity.Activity.CreateProfileImageUpdateActivity(userId, user.Name);
                await ActivityRepository.AddActivityAsync(activity);
            }
        }
    }
}
