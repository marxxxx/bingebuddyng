using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Activity.Commands
{
    public class AddReactionCommand : IRequest
    {
        public AddReactionCommand(string userId, ReactionType type, string activityId, string comment)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            ActivityId = activityId ?? throw new ArgumentNullException(nameof(activityId));
            Type = type;
            Comment = comment;
        }

        public string UserId { get; }
        public ReactionType Type { get; }
        public string ActivityId { get; }
        public string Comment { get; }

        public override string ToString()
        {
            return $"{{{nameof(UserId)}={UserId}, {nameof(Type)}={Type}, {nameof(ActivityId)}={ActivityId}, {nameof(Comment)}={Comment}}}";
        }
    }
}
