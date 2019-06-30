using BingeBuddyNg.Services.Venue;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Activity.Commands
{
    public class AddVenueActivityCommand : IRequest
    {
        public AddVenueActivityCommand(string userId, string message, VenueAction action, VenueModel venue)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Action = action;
            Venue = venue ?? throw new ArgumentNullException(nameof(venue));
        }

        public string UserId { get; }
        public string Message { get; }
        public VenueAction Action { get; }
        public VenueModel Venue { get;  }

    }
}
