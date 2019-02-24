using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using BingeBuddyNg.Api;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Linq;
using BingeBuddyNg.Services;
using System.IO;
using BingeBuddyNg.Functions;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.DrinkEvent;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.User;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace BingeBuddyNg.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        static IServiceProvider serviceProvider;
        
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            IServiceCollection services = new ServiceCollection();

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddEnvironmentVariables();
            
            services.AddApplicationServices(configurationBuilder.Build());

            serviceProvider = services.BuildServiceProvider();

        }

        [TestMethod]
        public async Task TestTranslations()
        {
            var translationService = serviceProvider.GetRequiredService<ITranslationService>();
            var translationValue = await translationService.GetTranslationAsync("NewVenue", "de");
            Assert.IsNotNull(translationValue);

        }

        [Ignore]
        [TestMethod]
        public async Task MigrateProfileImages()
        {

            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            StorageAccessService storageAccessService = serviceProvider.GetRequiredService<StorageAccessService>();
            var users = await userRepository.GetUsersAsync();
            foreach(var u in users)
            {
                await storageAccessService.AddQueueMessage(Shared.Constants.QueueNames.ProfileUpdate, new ProfileUpdateMessage(u.Id, u.ProfileImageUrl));
            }
            
        }

        [Ignore]
        [TestMethod]
        public async Task MigrateUserId()
        {
            StorageAccessService storageAccessService = serviceProvider.GetRequiredService<StorageAccessService>();
            PagedQueryResult<ActivityTableEntity> result = null;
            do
            {
                TableContinuationToken ct = result != null && result.ContinuationToken != null ? JsonConvert.DeserializeObject<TableContinuationToken>(result.ContinuationToken) : null;
                result = await storageAccessService.QueryTableAsync<ActivityTableEntity>("activity", null, 100, ct);
                var table = storageAccessService.GetTableReference("activity");
                foreach (var r in result.ResultPage)
                {
                    if (r.UserId == null)
                    {
                        r.UserId = r.Entity.UserId;

                        await table.ExecuteAsync(TableOperation.Merge(r));
                    }
                }
            } while (result.ContinuationToken != null);
        }


        [TestMethod]
        public async Task GetActivityFeedTest()
        {
            IActivityRepository activityRepository = serviceProvider.GetRequiredService<IActivityRepository>();
            var result = await activityRepository.GetActivityFeedAsync(new GetActivityFilterArgs(ActivityFilterOptions.None, pageSize: 1000));
            
            var timestamps = result.ResultPage.Select(p => p.Timestamp).ToList();
            var nextPage = await activityRepository.GetActivityFeedAsync(new GetActivityFilterArgs(ActivityFilterOptions.None, pageSize: 10, continuationToken: result.ContinuationToken));
            var timestampsNextPage = nextPage.ResultPage.Select(p => p.Timestamp).ToList();
            Assert.IsTrue(timestampsNextPage.All(t => timestamps.All(t1 => t1 < t)));
        }

        [TestMethod]
        public void ImageResizingLandscapeTest()
        {
            var strm = File.OpenRead(@".\Files\landscape.jpg");

            
            BingeBuddyNg.Functions.ImageResizingFunction.Run(strm, "landscape.jpg", out byte[] resizedData, new MockLogger());

            File.WriteAllBytes(@".\Files\landscape.resized.jpg", resizedData);


        }

        [TestMethod]
        public void ImageResizingPortraitTest()
        {
            var strm = File.OpenRead(@".\Files\portrait.jpg");


            BingeBuddyNg.Functions.ImageResizingFunction.Run(strm, "portrait.jpg", out byte[] resizedData, new MockLogger());

            File.WriteAllBytes(@".\Files\portrait.resized.jpg", resizedData);
        }

        [TestMethod]
        public async Task DrinkEventTests()
        {
            var drinkEventRepository = serviceProvider.GetRequiredService<IDrinkEventRepository>();
            await drinkEventRepository.CreateDrinkEventAsync(DateTime.UtcNow, DateTime.UtcNow.AddMinutes(15));
            var drinkEvent = await drinkEventRepository.FindCurrentDrinkEventAsync();
            Assert.IsNotNull(drinkEvent);
            string userId = Guid.NewGuid().ToString();
            drinkEvent.AddScoringUserId(userId);
            await drinkEventRepository.UpdateDrinkEventAsync(drinkEvent);
        }

        [TestMethod]
        public async Task DrinkStatsTests()
        {
            var service = serviceProvider.GetRequiredService<IUserStatsRepository>();
            await service.IncreaseScoreAsync("facebook|10219014482060805", 1);
        }
    }
}
