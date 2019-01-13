using BingeBuddyNg.Services.Configuration;
using BingeBuddyNg.Services.DTO;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using BingeBuddyNg.Services.Models.Generated;
using BingeBuddyNg.Services.Models.Venue;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services
{
    public class VenueService : IVenueService
    {
        private AppConfiguration configuration;
        private IHttpClientFactory httpClientFactory;
        private ILogger<VenueService> logger;

        public IUserRepository UserRepository { get; set; }
        public IActivityService ActivityService { get; set; }
        public IVenueUserRepository VenueUserRepository { get; }

        private const string BaseUrl = "https://api.foursquare.com/v2";

        public VenueService(
            IUserRepository userRepository, IActivityService activityService,
            IVenueUserRepository venueUserRepository,
            AppConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<VenueService> logger)
        {
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.ActivityService = activityService ?? throw new ArgumentNullException(nameof(activityService));
            this.VenueUserRepository = venueUserRepository ?? throw new ArgumentNullException(nameof(venueUserRepository));

            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<VenueModel>> SearchVenuesAsync(float latitude, float longitude)
        {
            var client = this.httpClientFactory.CreateClient();

            // force english format
            var englishCulture = CultureInfo.GetCultureInfo("en");
            string lalong = $"{latitude.ToString(englishCulture)},{longitude.ToString(englishCulture)}";
            string url = $"{BaseUrl}/venues/search?v=20190112&client_id={this.configuration.FourSquareApiClientKey}&client_secret={this.configuration.FourSquareApiClientSecret}&ll={lalong}";

            var response = await client.GetStringAsync(url);

            // convert
            var venueResult = JsonConvert.DeserializeObject<VenueRootObject>(response);

            var venues = venueResult.Response.Venues
                .Select(v => new VenueModel(v.Id, new Models.Location(v.Location.Lat, v.Location.Lng), v.Name, v.Location.Distance))
                .OrderBy(v => v.Distance)
                .ToList();
            return venues;
        }

        public async Task UpdateVenueForUserAsync(string userId, VenueModel venue)
        {
            var user = await this.UserRepository.FindUserAsync(userId);
            if (user.CurrentVenue != venue)
            {
                user.CurrentVenue = venue;

                var tasks = new[]
                {
                    this.UserRepository.UpdateUserAsync(user),
                    this.VenueUserRepository.AddUserToVenueAsync(venue.Id, venue.Name, userId, user.Name),
                    this.ActivityService.AddVenueActivityAsync(new AddVenueActivityDTO(user.Id,
                        $"Ich bin jetzt hier eingekehrt: {venue.Name}", venue))
                };

                await Task.WhenAll(tasks);
            }
        }

        public async Task ResetVenueForUserAsync(string userId)
        {
            var user = await this.UserRepository.FindUserAsync(userId);
            var currentVenue = user.CurrentVenue;
            
            if (currentVenue != null)
            {
                user.CurrentVenue = null;

                var tasks = new[]
                {
                    this.UserRepository.UpdateUserAsync(user),
                    this.VenueUserRepository.RemoveUserFromVenueAsync(currentVenue.Id, userId),
                    this.ActivityService.AddVenueActivityAsync(new AddVenueActivityDTO(user.Id,
                        $"Ich habe {currentVenue.Name} verlassen.", currentVenue))
                };

                await Task.WhenAll(tasks);
            }
        }
    }
}
