using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Drink;
using BingeBuddyNg.Services.DrinkEvent;
using BingeBuddyNg.Services.FriendsRequest;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Invitation;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.Venue;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BingeBuddyNg.Services
{
    public static class ApplicationServiceExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            string storageConnString = configuration.GetConnectionString("Storage");
            string googleApiKey = configuration.GetValue<string>("Credentials:GoogleApiKey");
            string webPushPrivateKey = configuration.GetValue<string>("Credentials:WebPushPrivateKey");
            string webPushPublicKey = configuration.GetValue<string>("Credentials:WebPushPublicKey");
            string fourSquareApiClientKey = configuration.GetValue<string>("Credentials:FourSquareApiClientKey");
            string fourSquareApiClientSecret = configuration.GetValue<string>("Credentials:FourSquareApiClientSecret");
            var appConfiguration = new AppConfiguration(storageConnString, googleApiKey, webPushPublicKey, webPushPrivateKey, 
                fourSquareApiClientKey, fourSquareApiClientSecret);
            services.AddSingleton(appConfiguration);
            services.AddLogging();

            services.AddMediatR(typeof(ActivityDTO).Assembly);

            // Add Application Services
            services.AddSingleton<StorageAccessService>();
            services.AddSingleton<IStorageAccessService, StorageAccessService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddSingleton<ITranslationService, TranslationService>();
            services.AddSingleton<ICacheService, CacheService>();

            services.AddScoped<IActivityRepository, ActivityRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserStatsRepository, UserStatsRepository>();
            services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
            services.AddScoped<IInvitationRepository, InvitationRepository>();

            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IDrinkEventRepository, DrinkEventRepository>();

            services.AddScoped<IVenueUserRepository, VenueUserRepository>();

            services.AddScoped<IDrinkRepository, DrinkRepository>();
            services.AddScoped<IUserStatsHistoryRepository, UserStatsHistoryRepository>();
            
        }
    }
}
