using System;
using System.Collections.Generic;

namespace BingeBuddyNg.Core.Game
{
    public interface IGameManager
    {
        event EventHandler<GameEndedEventArgs> GameEnded;

        void StartGame(Domain.Game game);

        Domain.Game GetGame(Guid gameId);

        int AddUserScore(Guid gameId, string userId, int score);

        IReadOnlyList<Domain.UserScore> GetGameResult(Guid gameId);

        Domain.UserScore FindWinner(Guid gameId);
    }
}