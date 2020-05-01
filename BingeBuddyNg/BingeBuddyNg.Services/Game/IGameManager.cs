using System;
using System.Collections.Generic;

namespace BingeBuddyNg.Services.Game
{
    public interface IGameManager
    {
        void CreateGame(Game game);

        Game GetGame(Guid gameId);

        int AddUserScore(Guid gameId, Guid userId, int score);

        IReadOnlyList<UserScore> GetGameResult(Guid gameId);
    }
}