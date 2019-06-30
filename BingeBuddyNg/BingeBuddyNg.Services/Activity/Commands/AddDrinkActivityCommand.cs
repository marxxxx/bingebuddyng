using BingeBuddyNg.Services.Drink;
using BingeBuddyNg.Services.Venue;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Activity.Commands
{
    public class AddDrinkActivityCommand : IRequest
    {
        public AddDrinkActivityCommand(string userId, string drinkId, DrinkType drinkType, string drinkName, double alcPrc, double volume, Location location, VenueModel venue)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            DrinkId = drinkId ?? throw new ArgumentNullException(nameof(drinkId));
            DrinkType = drinkType;
            DrinkName = drinkName ?? throw new ArgumentNullException(nameof(drinkName));
            AlcPrc = alcPrc;
            Volume = volume;
            Location = location;
            Venue = venue;
        }

        public string UserId { get; }
        public string DrinkId { get; }
        public DrinkType DrinkType { get; }
        public string DrinkName { get; }
        public double AlcPrc { get; }
        public double Volume { get; }
        public Location Location { get; }
        public VenueModel Venue { get; }

    }
}
