using MediatR;
using System;

namespace BingeBuddyNg.Services.Venue.Commands
{
    public class EnterVenueCommand : IRequest
    {
        public EnterVenueCommand(string userId, Venue venue)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Venue = venue ?? throw new ArgumentNullException(nameof(venue));
        }

        public string UserId { get; }
        public Venue Venue { get; }
    }
}
