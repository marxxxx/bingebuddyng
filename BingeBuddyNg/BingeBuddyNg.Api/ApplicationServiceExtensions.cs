using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Activity.DTO;
using BingeBuddyNg.Core.Drink;
using BingeBuddyNg.Core.DrinkEvent;
using BingeBuddyNg.Core.Game;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.Invitation;
using BingeBuddyNg.Core.Statistics;
using BingeBuddyNg.Core.Statistics.Queries;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Core.Venue;
using BingeBuddyNg.Infrastructure;
using BingeBuddyNg.Infrastructure.FourSquare;
using MediatR;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BingeBuddyNg.Api
{
    public static class ApplicationServiceExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddConfiguration(services, configuration);

            services.AddLogging();

            services.AddMediatR(typeof(ActivityDTO).Assembly);

            services.AddScoped<GetDrinksRankingQuery>();
            services.AddScoped<GetScoreRankingQuery>();

            // add infrastructure services
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddSingleton<ITranslationService, TranslationService>();
            services.AddSingleton<ICacheService, NoCacheService>();
            services.AddSingleton<IMessagingService, MessagingService>();
            services.AddSingleton<IFourSquareService, FourSquareService>();

            services.AddNotification(configuration);
            services.AddAzureSignalRIntegration(configuration);
            services.AddUtility(configuration);
            services.AddStorage(configuration);
            services.AddEventGrid(configuration);
            
            // add repositories
            services.AddSingleton<IActivityRepository, ActivityRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IInvitationRepository, InvitationRepository>();

            services.AddSingleton<IDrinkEventRepository, DrinkEventRepository>();

            services.AddSingleton<IVenueUserRepository, VenueUserRepository>();

            services.AddSingleton<IDrinkRepository, DrinkRepository>();
            services.AddScoped<UserStatisticUpdateService>();

            services.AddGame();
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

            services.AddTransient<IAddressDecodingService, AddressDecodingService>();
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

        public static void AddGame(this IServiceCollection services)
        {
            services.AddSingleton<GameRepository>();
        }
    }
}
