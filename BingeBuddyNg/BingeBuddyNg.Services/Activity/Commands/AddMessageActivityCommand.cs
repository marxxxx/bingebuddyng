using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Activity.Commands
{
    public class AddMessageActivityCommand :  IRequest
    {
        public AddMessageActivityCommand(string userId, string message)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public string UserId { get; }
        public string Message { get; }

        public override string ToString()
        {
            return $"{{{nameof(UserId)}={UserId}, {nameof(Message)}={Message}}}";
        }
    }
}
