using System;
using BingeBuddyNg.Services.User;

namespace BingeBuddyNg.Services.Activity.Domain
{
    public class RegistrationActivity : Activity
    {        
        public UserInfo RegistrationUser { get; private set; }

        private RegistrationActivity(string id, ActivityType type, DateTime timestamp, Location location, string userId, string userName, UserInfo registrationUser) : base(id, type, timestamp, location, userId, userName)
        {
            this.RegistrationUser = registrationUser ?? throw new ArgumentNullException(nameof(registrationUser));
        }

        public static RegistrationActivity Create(string userId, string userName, UserInfo registrationUser)
        {
            var timestamp = DateTime.Now;
            var id = ActivityId.Create(timestamp, userId);
            var activity = new RegistrationActivity(id.Value, ActivityType.Registration, timestamp, Location.Nowhere,
                userId, userName, registrationUser);
            return activity;
        }

        public static RegistrationActivity Create(string id, string userId, string userName, UserInfo registrationUser)
        {
            var activity = new RegistrationActivity(id, ActivityType.Registration, DateTime.UtcNow, Location.Nowhere,
                userId, userName, registrationUser);

            return activity;
        }
    }
}
