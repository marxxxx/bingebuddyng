using System;

namespace BingeBuddyNg.Services.Game
{
    public class UserScore
    {
        public UserScore(Guid userId, int score)
        {
            this.UserId = userId;
            this.Score = score;
        }

        public Guid UserId { get; }

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
            return $"{{{nameof(UserId)}={UserId.ToString()}, {nameof(Score)}={Score.ToString()}}}";
        }
    }
}
