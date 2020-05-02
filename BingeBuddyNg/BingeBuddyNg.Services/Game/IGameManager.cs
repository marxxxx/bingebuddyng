using System;
using System.Collections.Generic;

namespace BingeBuddyNg.Services.Game
{
    public interface IGameManager
    {
        event EventHandler<GameEndedEventArgs> GameEnded;

        void StartGame(Game game);

        Game GetGame(Guid gameId);

        int AddUserScore(Guid gameId, string userId, int score);

        IReadOnlyList<UserScore> GetGameResult(Guid gameId);

        UserScore GetWinner(Guid gameId);
    }
}