using System;
using BingeBuddyNg.Core.Venue;
using BingeBuddyNg.Core.Venue.DTO;

namespace BingeBuddyNg.Core.Activity.DTO
{
    public class AddVenueActivityDTO : AddActivityBaseDTO
    {
        public AddVenueActivityDTO(string userId, VenueDTO venue, VenueAction action)
        {
            this.UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            this.Venue = venue ?? throw new ArgumentNullException(nameof(venue));
            this.Location = venue.Location;
            this.Action = action;
        }

        public string UserId { get; set; }
        public VenueAction Action { get; }
    }
}
