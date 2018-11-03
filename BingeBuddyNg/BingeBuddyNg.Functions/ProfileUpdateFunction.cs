using BingeBuddyNg.Services;
using BingeBuddyNg.Services.DTO;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Messages;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BingeBuddyNg.Functions
{
    public static class ProfileUpdateFunction
    {
        private static HttpClient httpClient = new HttpClient();

        [FunctionName("ProfileUpdateFunction")]
        public static async Task Run([QueueTrigger("profile-update", Connection = "AzureWebJobsStorage")]string queueItem, ILogger log)
        {
            var message = JsonConvert.DeserializeObject<ProfileUpdateMessage>(queueItem);
            log.LogInformation($"Updating profile information for user {message.UserId} based on message {queueItem} ...");

            // we stick with poor man's DI for now
            IUserRepository userRepository = ServiceProviderBuilder.Instance.Value.GetRequiredService<IUserRepository>();
            StorageAccessService storageAccessService = ServiceProviderBuilder.Instance.Value.GetRequiredService<StorageAccessService>();

            using (var strm = await httpClient.GetStreamAsync(message.UserProfileImageUrl))
            {
                string fileName = $"{message.UserId}";
                await storageAccessService.SaveFileInBlobStorage("profileimg", fileName, strm);
            }

            log.LogInformation($"Successfully updated profile information for user {message.UserId}");
        }
    }
}
