using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Drink;
using BingeBuddyNg.Services.DrinkEvent;
using BingeBuddyNg.Services.FriendsRequest;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Infrastructure.EventGrid;
using BingeBuddyNg.Services.Infrastructure.Messaging;
using BingeBuddyNg.Services.Invitation;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.Venue;
using MediatR;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BingeBuddyNg.Services
{
    public static class ApplicationServiceExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddConfiguration(services, configuration);

            services.AddLogging();

            services.AddMediatR(typeof(ActivityDTO).Assembly);

            // add infrastructure services
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddSingleton<ITranslationService, TranslationService>();
            services.AddSingleton<ICacheService, NoCacheService>();
            services.AddSingleton<IMessagingService, MessagingService>();

            services.AddNotification(configuration);
            services.AddAzureSignalRIntegration(configuration);
            services.AddUtility(configuration);
            services.AddStorage(configuration);
            services.AddEventGrid(configuration);
            
            // add domain services
            services.AddScoped<IActivityRepository, ActivityRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserStatsRepository, UserStatsRepository>();
            services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
            services.AddScoped<IInvitationRepository, InvitationRepository>();

            services.AddScoped<IDrinkEventRepository, DrinkEventRepository>();

            services.AddScoped<IVenueUserRepository, VenueUserRepository>();

            services.AddScoped<IDrinkRepository, DrinkRepository>();
            services.AddScoped<IUserStatsHistoryRepository, UserStatsHistoryRepository>();
        }

        private static void AddConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            string eventHubConnectionString = configuration.GetConnectionString("EventHub");
            string fourSquareApiClientKey = configuration.GetValue<string>("FourSquareApiClientKey");
            string fourSquareApiClientSecret = configuration.GetValue<string>("FourSquareApiClientSecret");

            services.AddSingleton(new FourSquareConfiguration(fourSquareApiClientKey, fourSquareApiClientSecret));
            services.AddSingleton(new MessagingConfiguration(eventHubConnectionString));
        }

        public static void AddNotification(this IServiceCollection services, IConfiguration configuration)
        {
            string webPushPrivateKey = configuration.GetValue<string>("WebPushPrivateKey");
            string webPushPublicKey = configuration.GetValue<string>("WebPushPublicKey");
            services.AddSingleton(new WebPushConfiguration(webPushPublicKey, webPushPrivateKey));

            services.AddTransient<INotificationService, NotificationService>();
        }


        public static void AddAzureSignalRIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("SignalR");
            var serviceManager = new ServiceManagerBuilder()
                .WithOptions(o => o.ConnectionString = connectionString)
                .Build();
            services.AddSingleton(serviceManager);
        }

        public static void AddUtility(this IServiceCollection services, IConfiguration configuration)
        {
            string googleApiKey = configuration.GetValue<string>("GoogleApiKey");
            services.AddSingleton(new GoogleApiConfiguration(googleApiKey));

            services.AddTransient<IUtilityService, UtilityService>();
        }

        public static void AddStorage(this IServiceCollection services, IConfiguration configuration)
        {
            string storageConnectionString = configuration.GetConnectionString("Storage");
            services.AddSingleton(new StorageConfiguration(storageConnectionString));

            services.AddSingleton<IStorageAccessService, StorageAccessService>();
        }

        public static void AddEventGrid(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("EventGrid").Get<EventGridConfiguration>();
            services.AddSingleton(config);
            services.AddSingleton<IEventGridService, EventGridService>();
        }
    }
}
