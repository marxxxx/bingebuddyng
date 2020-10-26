using System;

namespace BingeBuddyNg.Core.User
{
    public class FriendshipException : Exception
    {
        public FriendshipException(string message) : base(message)
        {
        }
    }
}