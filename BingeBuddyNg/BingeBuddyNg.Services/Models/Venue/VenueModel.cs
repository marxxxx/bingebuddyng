using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Models.Venue
{
    public class VenueModel
    {
        public VenueModel()
        {
        }

        public VenueModel(string id, Location location, string name, int distance)
        {
            this.Id = id ?? throw new ArgumentNullException(nameof(id));
            Location = location ?? throw new ArgumentNullException(nameof(location));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Distance = distance;
        }

        public string Id { get; set; }
        public Location Location { get; set; }
        public string Name { get; set; }
        public int Distance { get; set; }
    }
}
