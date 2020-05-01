using BingeBuddyNg.Services.User;
using System;
using System.Collections.Generic;

namespace BingeBuddyNg.Services.Game.Queries
{
    public class GameStatusDTO
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public List<UserInfo> Players { get; set; }

        public List<UserScore> Scores { get; set; }
    }
}
