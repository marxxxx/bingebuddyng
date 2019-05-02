using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.User
{
    public class CreateOrUpdateUserResult
    {
        public bool IsNewUser {get;}

        public CreateOrUpdateUserResult(bool isNewUser)
        {
            this.IsNewUser = isNewUser;
        }
    
    }
}
