using MediatR;
using System;

namespace BingeBuddyNg.Services.Venue.Commands
{
    public class LeaveVenueCommand : IRequest
    {
        public LeaveVenueCommand(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        public string UserId { get; }
    }
}
