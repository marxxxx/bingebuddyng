using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Activity.Commands
{
    public class AddReactionCommand : IRequest
    {
        public AddReactionCommand(ReactionType type, string activityId, string comment)
        {
            Type = type;
            ActivityId = activityId ?? throw new ArgumentNullException(nameof(activityId));
            Comment = comment;
        }

        public ReactionType Type { get; }
        public string ActivityId { get; }
        public string Comment { get; }

        public override string ToString()
        {
            return $"{{{nameof(Type)}={Type}, {nameof(ActivityId)}={ActivityId}, {nameof(Comment)}={Comment}}}";
        }
    }
}
