using System;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Activity.Commands;
using BingeBuddyNg.Core.Activity.Queries;
using BingeBuddyNg.Core.Calculation;
using BingeBuddyNg.Core.DrinkEvent;
using BingeBuddyNg.Core.Statistics;
using BingeBuddyNg.Core.Statistics.Commands;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Core.User.Queries;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.DrinkEvent;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Infrastructure.EventGrid;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.User.Queries;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.DependencyInjection;

namespace BingeBuddyNg.Functions
{
    public static class ApplicationServiceExtensions
    {
        public static void AddBingeBuddy(this IServiceCollection services)
        {
            // Domain Services
            services.AddScoped<IActivityRepository, ActivityRepository>();
            services.AddUser();
            services.AddScoped<CalculationService>();
            services.AddScoped<IDrinkEventRepository, DrinkEventRepository>();
            
            // Commands & Queries
            services.AddScoped<UpdateRankingCommand>();
            services.AddScoped<UpdateStatisticsCommand>();
            services.AddScoped<IncreaseScoreCommand>();
            services.AddScoped<DeleteActivityFromPersonalizedFeedCommand>();
            services.AddScoped<DistributeActivityToPersonalizedFeedCommand>();
            services.AddScoped<GetUserActivitiesQuery>();

            // Infrastructure
            services.AddHttpClient();
            services.AddNotification();
            services.AddAzureSignalRIntegration();
            services.AddUtility();
            services.AddStorage();
            services.AddEventGrid();

            services.AddTransient<ICacheService, NoCacheService>();
            services.AddSingleton<ITranslationService, TranslationService>();
            services.AddTransient<IAddressDecodingService, AddressDecodingService>();
        }

        public static void AddUser(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<SearchUsersQuery>();
            services.AddScoped<GetAllUserIdsQuery>();
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
