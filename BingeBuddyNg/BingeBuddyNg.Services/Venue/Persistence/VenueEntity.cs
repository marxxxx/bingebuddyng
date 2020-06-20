using System;
using System.Collections.Generic;
using System.Text;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Services.Activity;

namespace BingeBuddyNg.Services.Venue.Persistence
{
    public class VenueEntity
    {
        public string Id { get; set; }

        public Location Location { get; set; }

        public string Name { get; set; }

        public int Distance { get; set; }
    }
}
