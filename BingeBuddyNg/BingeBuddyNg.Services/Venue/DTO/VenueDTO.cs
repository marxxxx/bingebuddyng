using BingeBuddyNg.Core.Activity.Domain;

namespace BingeBuddyNg.Core.Venue.DTO
{
    public class VenueDTO
    {
        public string Id { get; set; }
        public Location Location { get; set; }
        public string Name { get; set; }
        public int Distance { get; set; }
    }
}
