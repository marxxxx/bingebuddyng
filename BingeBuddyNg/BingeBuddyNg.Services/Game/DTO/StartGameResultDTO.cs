using System;

namespace BingeBuddyNg.Core.Game.DTO
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
