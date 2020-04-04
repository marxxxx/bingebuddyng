using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Calculation;
using BingeBuddyNg.Services.DrinkEvent;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BingeBuddyNg.Functions
{
    public static class ApplicationServiceExtensions
    {
        public static void AddBingeBuddy(this IServiceCollection services)
        {
            // Domain Services
            services.AddSingleton<ITranslationService, TranslationService>();
            services.AddTransient<IActivityRepository, ActivityRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUtilityService, UtilityService>();

            services.AddTransient<ICalculationService, CalculationService>();
            services.AddTransient<IUserStatsRepository, UserStatsRepository>();
            services.AddTransient<IDrinkEventRepository, DrinkEventRepository>();
            services.AddTransient<IUserStatisticsService, UserStatisticsService>();
            services.AddTransient<IUserStatsHistoryRepository, UserStatsHistoryRepository>();

            services.AddTransient<ICacheService, NoCacheService>();

            // Infrastructure
            services.AddHttpClient();
            services.AddNotification();
            services.AddAzureSignalRIntegration();
            services.AddUtility();
            services.AddStorage();
        }

        public static void AddNotification(this IServiceCollection services)
        {
            string webPushPublicKey = Environment.GetEnvironmentVariable("WebPushPublicKey", EnvironmentVariableTarget.Process);
            string webPushPrivateKey = Environment.GetEnvironmentVariable("WebPushPrivateKey", EnvironmentVariableTarget.Process);
            services.AddSingleton(new WebPushConfiguration(webPushPublicKey, webPushPrivateKey));

            services.AddTransient<INotificationService, NotificationService>();
        }

        public static void AddAzureSignalRIntegration(this IServiceCollection services)
        {
            string connectionString = Environment.GetEnvironmentVariable("SignalRConnectionString");
            var serviceManager = new ServiceManagerBuilder()
                .WithOptions(o =>
                {
                    o.ConnectionString = connectionString;
                })
                .Build();
            services.AddSingleton(serviceManager);
        }

        public static void AddUtility(this IServiceCollection services)
        {
            string googleApiKey = Environment.GetEnvironmentVariable("GoogleApiKey", EnvironmentVariableTarget.Process);
            services.AddSingleton(new GoogleApiConfiguration(googleApiKey));

            services.AddTransient<IUtilityService, UtilityService>();
        }

        public static void AddStorage(this IServiceCollection services)
        {
            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);

            services.AddSingleton(new StorageConfiguration(storageConnectionString));

            services.AddSingleton<IStorageAccessService, StorageAccessService>();
        }
    }
}
