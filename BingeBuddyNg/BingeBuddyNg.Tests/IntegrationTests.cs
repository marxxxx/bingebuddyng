using BingeBuddyNg.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using BingeBuddyNg.Api;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using BingeBuddyNg.Services.DTO;
using System.Linq;
using BingeBuddyNg.Services;

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
        public async Task GetActivityFeedTest()
        {
            IActivityRepository activityRepository = serviceProvider.GetRequiredService<IActivityRepository>();
            var result = await activityRepository.GetActivityFeedAsync(new GetActivityFilterArgs(false, 1000));
            
            var timestamps = result.ResultPage.Select(p => p.Timestamp).ToList();
            var nextPage = await activityRepository.GetActivityFeedAsync(new GetActivityFilterArgs(false, 10, result.ContinuationToken));
            var timestampsNextPage = nextPage.ResultPage.Select(p => p.Timestamp).ToList();
            Assert.IsTrue(timestampsNextPage.All(t => timestamps.All(t1 => t1 < t)));
        }
    }
}
