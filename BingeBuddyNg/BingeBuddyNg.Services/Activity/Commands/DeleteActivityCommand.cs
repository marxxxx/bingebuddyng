using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Activity.Commands
{
    public class DeleteActivityCommand : IRequest
    {
        public string UserId { get; }
        public string ActivityId { get; }

        public DeleteActivityCommand(string userId, string activityId)
        {
            this.UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            this.ActivityId = activityId ?? throw new ArgumentNullException(nameof(activityId));
        }
    }
}
