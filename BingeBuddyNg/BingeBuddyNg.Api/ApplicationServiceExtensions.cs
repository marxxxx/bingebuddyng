using BingeBuddyNg.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.DrinkEvent;
using BingeBuddyNg.Services.FriendsRequest;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Invitation;
using BingeBuddyNg.Services.Ranking;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.Venue;
using BingeBuddyNg.Services.Drink;
using BingeBuddyNg.Services.Statistics;
using MediatR;
using System.Reflection;

namespace BingeBuddyNg.Api
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

            services.AddMediatR(typeof(Activity).Assembly);

            // Add Application Services
            services.AddSingleton<StorageAccessService>();
            services.AddSingleton<IStorageAccessService, StorageAccessService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddSingleton<ITranslationService, TranslationService>();

            services.AddScoped<IActivityRepository, ActivityRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserStatsRepository, UserStatsRepository>();
            services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
            services.AddScoped<IInvitationRepository, InvitationRepository>();

            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IFriendRequestService, FriendRequestService>();
            services.AddScoped<IUserRankingService, UserRankingService>();
            services.AddScoped<IInvitationService, InvitationService>();
            services.AddScoped<IDrinkEventRepository, DrinkEventRepository>();

            services.AddScoped<IVenueService, VenueService>();
            services.AddScoped<IVenueUserRepository, VenueUserRepository>();
            services.AddScoped<IVenueRankingService, VenueRankingService>();

            services.AddScoped<IDrinkRepository, DrinkRepository>();
            services.AddScoped<IUserStatsHistoryRepository, UserStatsHistoryRepository>();
            services.AddScoped<IUserService, UserService>();
        }
    }
}
