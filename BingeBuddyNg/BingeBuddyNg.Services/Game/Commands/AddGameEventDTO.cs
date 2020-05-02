using System;

namespace BingeBuddyNg.Services.Game.Commands
{
    public class AddGameEventDTO
    {
        public Guid GameId { get; }

        public int Count { get; }
    }
}
