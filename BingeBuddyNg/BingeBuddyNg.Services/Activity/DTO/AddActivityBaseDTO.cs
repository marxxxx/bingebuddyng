using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.Venue.DTO;

namespace BingeBuddyNg.Core.Activity.DTO
{
    public class AddActivityBaseDTO
    {
        public Location Location { get; set; }
        public VenueDTO Venue { get; set; }
    }
}
