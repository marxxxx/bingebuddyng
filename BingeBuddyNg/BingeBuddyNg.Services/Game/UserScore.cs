using System;

namespace BingeBuddyNg.Services.Game
{
    public class UserScore
    {
        public UserScore(Guid userId, int currentScore)
        {
            this.UserId = userId;
            this.CurrentScore = currentScore;
        }

        public Guid UserId { get; }

        public int CurrentScore { get; }

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
            return $"{{{nameof(UserId)}={UserId.ToString()}, {nameof(CurrentScore)}={CurrentScore.ToString()}}}";
        }
    }
}
