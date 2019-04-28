using BingeBuddyNg.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using System;

namespace BingeBuddyNg.Functions
{
    public class ProfileUpdateFunction
    {
        public ProfileUpdateFunction(StorageAccessService storageAccessService, IHttpClientFactory httpClientFactory)
        {
            this.StorageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
            this.HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public StorageAccessService StorageAccessService { get; }
        public IHttpClientFactory HttpClientFactory { get; }

        [FunctionName("ProfileUpdateFunction")]
        public async Task Run([QueueTrigger(Shared.Constants.QueueNames.ProfileUpdate, Connection = "AzureWebJobsStorage")]string queueItem, ILogger log)
        {
            var message = JsonConvert.DeserializeObject<ProfileUpdateMessage>(queueItem);
            log.LogInformation($"Updating profile information for user {message.UserId} based on message {queueItem} ...");

            var httpClient = HttpClientFactory.CreateClient();
            using (var strm = await httpClient.GetStreamAsync(message.UserProfileImageUrl))
            {
                string fileName = $"{message.UserId}";
                await StorageAccessService.SaveFileInBlobStorage("profileimg", fileName, strm);
            }

            log.LogInformation($"Successfully updated profile information for user {message.UserId}");
        }
    }
}
