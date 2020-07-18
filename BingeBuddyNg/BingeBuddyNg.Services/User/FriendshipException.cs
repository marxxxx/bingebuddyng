using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Core.User
{
    public class FriendshipException : Exception
    {
        public FriendshipException(string message) : base(message)
        {
        }
    }
}
