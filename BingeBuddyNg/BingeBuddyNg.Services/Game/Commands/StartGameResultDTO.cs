using System;

namespace BingeBuddyNg.Services.Game
{
    public class StartGameResultDTO
    {
        public StartGameResultDTO(Guid gameId)
        {
            GameId = gameId;
        }

        public Guid GameId { get; set; }
    }

}
