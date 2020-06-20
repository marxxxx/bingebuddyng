using BingeBuddyNg.Core.User;
using BingeBuddyNg.Services.User;

namespace BingeBuddyNg.Services.Game.Persistence
{
    public class UserScoreInfo
    {
        public UserInfo User { get; set; }

        public int Score { get; set; }
    }
}
