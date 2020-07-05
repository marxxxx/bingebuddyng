using System;

namespace BingeBuddyNg.Core.User.Messages
{
    public class UserRenamedMessage
    {
        public UserRenamedMessage()
        {
        }

        public UserRenamedMessage(string userId, string oldUserName, string newUserName)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            OldUserName = oldUserName ?? throw new ArgumentNullException(nameof(oldUserName));
            NewUserName = newUserName ?? throw new ArgumentNullException(nameof(newUserName));
        }

        public string UserId { get; set; }
        public string OldUserName { get; set; }
        public string NewUserName { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(UserId)}={UserId}, {nameof(OldUserName)}={OldUserName}, {nameof(NewUserName)}={NewUserName}}}";
        }
    }
}
