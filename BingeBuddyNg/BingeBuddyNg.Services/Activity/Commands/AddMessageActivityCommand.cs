using BingeBuddyNg.Services.Venue;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Activity.Commands
{
    public class AddMessageActivityCommand :  IRequest<string>
    {
        public AddMessageActivityCommand(string userId, string message, Location location, VenueModel venue)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Location = location;
            Venue = venue;
        }

        public string UserId { get; }
        public string Message { get; }
        public Location Location { get; }
        public VenueModel Venue { get; }
    }
}
