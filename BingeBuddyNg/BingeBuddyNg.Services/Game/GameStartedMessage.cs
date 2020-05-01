using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Game
{
    public class GameStartedMessage
    {
        public GameStartedMessage(Guid gameId, string title, Guid[] userIds)
        {
            GameId = gameId;
            Title = title ?? throw new ArgumentNullException(nameof(title));
            UserIds = userIds ?? throw new ArgumentNullException(nameof(userIds));
        }

        public Guid GameId { get; }

        public string Title { get; }

        public Guid[] UserIds { get;  }

        public override string ToString()
        {
            return $"{{{nameof(GameId)}={GameId.ToString()}, {nameof(Title)}={Title}, {nameof(UserIds)}={UserIds}}}";
        }
    }
}
