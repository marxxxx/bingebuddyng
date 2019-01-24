﻿using BingeBuddyNg.Services.Models;
using BingeBuddyNg.Services.Models.Venue;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.DTO
{
    public class AddVenueActivityDTO : AddActivityBaseDTO
    {
        public AddVenueActivityDTO(string userId, string message, VenueModel venue)
        {
            this.UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            this.Message = message ?? throw new ArgumentNullException(nameof(message));
            this.Venue = venue ?? throw new ArgumentNullException(nameof(venue));
            this.Location = venue.Location;
        }

        public string UserId { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(UserId)}={UserId}, {nameof(Message)}={Message}, {nameof(Location)}={Location}, {nameof(Venue)}={Venue}}}";
        }
    }
}