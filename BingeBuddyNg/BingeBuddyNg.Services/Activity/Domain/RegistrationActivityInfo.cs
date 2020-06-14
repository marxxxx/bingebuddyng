using System;
using BingeBuddyNg.Services.User;

namespace BingeBuddyNg.Services.Activity.Domain
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
