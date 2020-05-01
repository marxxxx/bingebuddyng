using System;

namespace BingeBuddyNg.Services.Game
{
    public class GameUpdateReceivedMessage
    {
        public GameUpdateReceivedMessage(Guid gameId, Guid userId, int currentScore)
        {
            GameId = gameId;
            UserId = userId;
            CurrentScore = currentScore;
        }

        public Guid GameId { get; }
        public Guid UserId { get; }
        public int CurrentScore { get; }

        public override string ToString()
        {
            return $"{{{nameof(GameId)}={GameId.ToString()}, {nameof(UserId)}={UserId.ToString()}, {nameof(CurrentScore)}={CurrentScore.ToString()}}}";
        }
    }
}
