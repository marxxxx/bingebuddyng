﻿using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;
using System;

namespace BingeBuddyNg.Services.Ranking
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