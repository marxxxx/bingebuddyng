using System;
using BingeBuddyNg.Core.User.Persistence;

namespace BingeBuddyNg.Core.Activity.Domain
{
    public class RegistrationActivityInfo
    {
        public UserInfo RegistrationUser { get; private set; }

        public RegistrationActivityInfo(UserInfo registrationUser)
        {
            this.RegistrationUser = registrationUser ?? throw new ArgumentNullException(nameof(registrationUser));
        }
    }
}