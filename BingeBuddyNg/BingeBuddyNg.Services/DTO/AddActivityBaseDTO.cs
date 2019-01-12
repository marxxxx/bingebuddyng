using BingeBuddyNg.Services.Models;
using BingeBuddyNg.Services.Models.Venue;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.DTO
{
    public class AddActivityBaseDTO
    {
        public Location Location { get; set; }
        public VenueModel Venue { get; set; }
    }
}
