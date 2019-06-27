using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Activity.Commands
{
    public class AddMessageActivityCommand :  IRequest
    {
        public AddMessageActivityCommand(string message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public string Message { get; }

        public override string ToString()
        {
            return $"{{{nameof(Message)}={Message}}}";
        }
    }
}
