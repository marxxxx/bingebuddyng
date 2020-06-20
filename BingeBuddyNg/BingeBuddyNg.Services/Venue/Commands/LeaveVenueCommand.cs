using MediatR;
using System;

namespace BingeBuddyNg.Core.Venue.Commands
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
