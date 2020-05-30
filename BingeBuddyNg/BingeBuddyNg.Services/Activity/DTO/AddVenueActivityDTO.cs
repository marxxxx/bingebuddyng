using System;
using BingeBuddyNg.Services.Venue;

namespace BingeBuddyNg.Services.Activity
{
    public class AddVenueActivityDTO : AddActivityBaseDTO
    {
        public AddVenueActivityDTO(string userId, string message, Venue.Venue venue, VenueAction action)
        {
            this.UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            this.Message = message ?? throw new ArgumentNullException(nameof(message));
            this.Venue = venue ?? throw new ArgumentNullException(nameof(venue));
            this.Location = venue.Location;
            this.Action = action;
        }

        public string UserId { get; set; }
        public string Message { get; set; }
        public VenueAction Action { get; }

        public override string ToString()
        {
            return $"{{{nameof(UserId)}={UserId}, {nameof(Message)}={Message}, {nameof(Action)}={Action}, {nameof(Location)}={Location}, {nameof(Venue)}={Venue}}}";
        }
    }
}
