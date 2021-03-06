﻿using System;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Calculation;
using BingeBuddyNg.Core.DrinkEvent;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.Statistics;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Infrastructure;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.DependencyInjection;

namespace BingeBuddyNg.Functions
{
    public static class ApplicationServiceExtensions
    {
        public static void AddBingeBuddy(this IServiceCollection services)
        {
            // Services
            services.AddScoped<IActivityRepository, ActivityRepository>();
            services.AddUser();
            services.AddScoped<CalculationService>();
            services.AddScoped<IDrinkEventRepository, DrinkEventRepository>();
            services.AddScoped<UserStatisticUpdateService>();

            // Infrastructure
            services.AddHttpClient();
            services.AddNotification();
            services.AddAzureSignalRIntegration();
            services.AddUtility();
            services.AddStorage();
            services.AddEventGrid();

            services.AddScoped<IMonitoringRepository, MonitoringRepository>();
            services.AddScoped<ICacheService, NoCacheService>();
            services.AddSingleton<ITranslationService, TranslationService>();
            services.AddScoped<IAddressDecodingService, AddressDecodingService>();
        }

        public static void AddUser(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
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

            services.AddTransient<IAddressDecodingService, AddressDecodingService>();
        }

        public static void AddStorage(this IServiceCollection services)
        {
            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);

            services.AddSingleton(new StorageConfiguration(storageConnectionString));

            services.AddSingleton<IStorageAccessService, StorageAccessService>();
        }

        public static void AddEventGrid(this IServiceCollection services)
        {
            var endpoint = Environment.GetEnvironmentVariable("EventGrid__Endpoint", EnvironmentVariableTarget.Process);
            var topicKey = Environment.GetEnvironmentVariable("EventGrid__TopicKey", EnvironmentVariableTarget.Process);

            var config = new EventGridConfiguration() { Endpoint = endpoint, TopicKey = topicKey };

            services.AddSingleton(config);
            services.AddSingleton<IEventGridService, EventGridService>();
        }
    }
}