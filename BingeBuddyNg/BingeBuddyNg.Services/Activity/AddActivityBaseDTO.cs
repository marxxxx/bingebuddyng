﻿using BingeBuddyNg.Services.Venue;

namespace BingeBuddyNg.Services.Activity
{
    public class AddActivityBaseDTO
    {
        public Location Location { get; set; }
        public Venue.Venue Venue { get; set; }
    }
}
