using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Venue.Commands
{
    public class EnterVenueCommand : IRequest
    {
        public EnterVenueCommand(string userId, VenueModel venue)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Venue = venue ?? throw new ArgumentNullException(nameof(venue));
        }

        public string UserId { get; }
        public VenueModel Venue { get; }
    }
}
