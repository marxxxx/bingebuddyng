using BingeBuddyNg.Services;
using BingeBuddyNg.Services.Configuration;
using BingeBuddyNg.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var appConfiguration = new AppConfiguration(storageConnString, googleApiKey, webPushPublicKey, webPushPrivateKey);
            services.AddSingleton(appConfiguration);

            // Add Application Services
            services.AddScoped<StorageAccessService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IActivityRepository, ActivityRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUserStatsRepository, UserStatsRepository>();
            services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
            services.AddScoped<IFriendRequestService, FriendRequestService>();
            services.AddScoped<IRankingService, RankingService>();
        }
    }
}
