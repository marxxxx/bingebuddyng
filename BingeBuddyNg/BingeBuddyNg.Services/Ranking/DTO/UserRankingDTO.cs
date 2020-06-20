using System;
using BingeBuddyNg.Core.Statistics.DTO;
using BingeBuddyNg.Services.User;

namespace BingeBuddyNg.Core.Ranking.DTO
{
    public class UserRankingDTO
    {
        public UserRankingDTO()
        {
        }

        public UserRankingDTO(UserInfoDTO user, UserStatisticsDTO statistics)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            Statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));
        }

        public UserInfoDTO User { get; set; }
        public UserStatisticsDTO Statistics { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(User)}={User}, {nameof(Statistics)}={Statistics}}}";
        }
    }
}
