﻿using BingeBuddyNg.Services;
using BingeBuddyNg.Services.Calculation;
using BingeBuddyNg.Services.Configuration;
using BingeBuddyNg.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BingeBuddyNg.Functions
{
    /// <summary>
    /// Builds the dependency injection container and registers all services.
    /// </summary>
    public static class ServiceProviderBuilder 
    {
        public static Lazy<IServiceProvider> Instance = new Lazy<IServiceProvider>(() => BuildServiceProvider(), false);

        private static IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);
            string googleApiKey = Environment.GetEnvironmentVariable("GoogleApiKey", EnvironmentVariableTarget.Process);
            string webPushPublicKey = Environment.GetEnvironmentVariable("WebPushPublicKey", EnvironmentVariableTarget.Process);
            string webPushPrivateKey = Environment.GetEnvironmentVariable("WebPushPrivateKey", EnvironmentVariableTarget.Process);
            string fourSquareApiClientKey = Environment.GetEnvironmentVariable("FourSquareApiClientKey", EnvironmentVariableTarget.Process);
            string fourSquareApiClientSecret = Environment.GetEnvironmentVariable("FourSquareApiClientSecret", EnvironmentVariableTarget.Process);


            services.AddHttpClient();

            var configuration = new AppConfiguration(storageConnectionString, googleApiKey, webPushPublicKey, webPushPrivateKey,
                fourSquareApiClientKey, fourSquareApiClientSecret);
            services.AddSingleton(configuration);
            services.AddScoped<StorageAccessService>();
            services.AddScoped<IActivityRepository, ActivityRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUtilityService, UtilityService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICalculationService, CalculationService>();
            services.AddScoped<IUserStatsRepository, UserStatsRepository>();
            services.AddScoped<IDrinkEventRepository, DrinkEventRepository>();

            return services.BuildServiceProvider();
        }
    }
}
