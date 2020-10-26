using System;

namespace BingeBuddyNg.Core.Game
{
    public class GameStartedMessage
    {
        public GameStartedMessage(Guid gameId, string title, string[] userIds)
        {
            GameId = gameId;
            Title = title ?? throw new ArgumentNullException(nameof(title));
            UserIds = userIds ?? throw new ArgumentNullException(nameof(userIds));
        }

        public Guid GameId { get; }

        public string Title { get; }

        public string[] UserIds { get; }

        public override string ToString()
        {
            return $"{{{nameof(GameId)}={GameId.ToString()}, {nameof(Title)}={Title}, {nameof(UserIds)}={UserIds}}}";
        }
    }
}