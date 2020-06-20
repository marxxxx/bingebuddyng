using BingeBuddyNg.Core.Venue.DTO;
using MediatR;
using System;

namespace BingeBuddyNg.Core.Venue.Commands
{
    public class EnterVenueCommand : IRequest
    {
        public EnterVenueCommand(string userId, VenueDTO venue)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Venue = venue ?? throw new ArgumentNullException(nameof(venue));
        }

        public string UserId { get; }
        public VenueDTO Venue { get; }
    }
}
