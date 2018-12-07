using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Models
{
    public class UserRanking
    {
        public UserRanking()
        {
        }

        public UserRanking(UserInfo user, UserStatistics statistics)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            Statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));
        }

        public UserInfo User { get; set; }
        public UserStatistics Statistics { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(User)}={User}, {nameof(Statistics)}={Statistics}}}";
        }
    }
}
