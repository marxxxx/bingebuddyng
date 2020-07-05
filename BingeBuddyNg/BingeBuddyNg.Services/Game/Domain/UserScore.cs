using System;

namespace BingeBuddyNg.Core.Game.Domain
{
    public class UserScore
    {
        public UserScore(string userId, int score)
        {
            this.UserId = userId;
            this.Score = score;
        }

        public string UserId { get; }

        public int Score { get; }

        public override bool Equals(object obj)
        {
            return obj is UserScore score &&
                   UserId.Equals(score.UserId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserId);
        }

        public override string ToString()
        {
            return $"{{{nameof(UserId)}={UserId}, {nameof(Score)}={Score.ToString()}}}";
        }
    }
}
