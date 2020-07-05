using System;
using System.Collections.Generic;
using BingeBuddyNg.Core.Activity.Domain;

namespace BingeBuddyNg.Core.Venue
{
    public class Venue
    {
        public Venue()
        {
        }

        public Venue(string id, Location location, string name, int distance)
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

        public override bool Equals(object obj)
        {
            var model = obj as Venue;
            return model != null &&
                   Id == model.Id;
        }

        public override int GetHashCode()
        {
            return 2108858624 + EqualityComparer<string>.Default.GetHashCode(Id);
        }
    }
}
