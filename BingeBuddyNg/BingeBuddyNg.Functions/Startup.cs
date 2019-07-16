﻿using BingeBuddyNg.Functions;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Calculation;
using BingeBuddyNg.Services.DrinkEvent;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
namespace BingeBuddyNg.Functions
{
    // Implement IWebJobStartup interface.

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);
            string googleApiKey = Environment.GetEnvironmentVariable("GoogleApiKey", EnvironmentVariableTarget.Process);
            string webPushPublicKey = Environment.GetEnvironmentVariable("WebPushPublicKey", EnvironmentVariableTarget.Process);
            string webPushPrivateKey = Environment.GetEnvironmentVariable("WebPushPrivateKey", EnvironmentVariableTarget.Process);
            string fourSquareApiClientKey = Environment.GetEnvironmentVariable("FourSquareApiClientKey", EnvironmentVariableTarget.Process);
            string fourSquareApiClientSecret = Environment.GetEnvironmentVariable("FourSquareApiClientSecret", EnvironmentVariableTarget.Process);

            services.AddHttpClient();
            services.AddMemoryCache();

            var configuration = new AppConfiguration(storageConnectionString, googleApiKey, webPushPublicKey, webPushPrivateKey,
                fourSquareApiClientKey, fourSquareApiClientSecret);
            services.AddSingleton(configuration);
            services.AddSingleton<StorageAccessService>();
            services.AddSingleton<IStorageAccessService, StorageAccessService>();
            services.AddSingleton<ITranslationService, TranslationService>();
            services.AddTransient<IActivityRepository, ActivityRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUtilityService, UtilityService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<ICalculationService, CalculationService>();
            services.AddTransient<IUserStatsRepository, UserStatsRepository>();
            services.AddTransient<IDrinkEventRepository, DrinkEventRepository>();
            services.AddTransient<IUserStatisticsService, UserStatisticsService>();
            services.AddTransient<IUserStatsHistoryRepository, UserStatsHistoryRepository>();
            services.AddTransient<IUtilityService, UtilityService>();
            services.AddTransient<ICacheService, CacheService>();
        }
    }
}
