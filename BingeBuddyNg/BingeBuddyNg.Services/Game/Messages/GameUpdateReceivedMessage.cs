using System;

namespace BingeBuddyNg.Core.Game
{
    public class GameUpdateReceivedMessage
    {
        public GameUpdateReceivedMessage(Guid gameId, string userId, int currentScore)
        {
            GameId = gameId;
            UserId = userId;
            CurrentScore = currentScore;
        }

        public Guid GameId { get; }
        public string UserId { get; }
        public int CurrentScore { get; }

        public override string ToString()
        {
            return $"{{{nameof(GameId)}={GameId.ToString()}, {nameof(UserId)}={UserId.ToString()}, {nameof(CurrentScore)}={CurrentScore.ToString()}}}";
        }
    }
}
