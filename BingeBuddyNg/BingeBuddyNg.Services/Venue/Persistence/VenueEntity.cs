using BingeBuddyNg.Core.Activity.Domain;

namespace BingeBuddyNg.Core.Venue.Persistence
{
    public class VenueEntity
    {
        public string Id { get; set; }

        public Location Location { get; set; }

        public string Name { get; set; }

        public int Distance { get; set; }
    }
}