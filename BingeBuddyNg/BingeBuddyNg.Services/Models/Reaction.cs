using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Models
{
    public class Reaction
    {
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }

        public Reaction()
        {
        }

        public Reaction(string userId, string userName, string userProfileImageUrl) 
            : this(DateTime.UtcNow, userId, userName)
        {

        }

        public Reaction(DateTime timestamp, string userId, string userName)
        {
            this.Timestamp = timestamp;
            this.UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            this.UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        }

        public override string ToString()
        {
            return $"{{{nameof(Timestamp)}={Timestamp}, {nameof(UserId)}={UserId}, {nameof(UserName)}={UserName}}}";
        }
    }
}
