using BingeBuddyNg.Services.Drink;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Activity.Commands
{
    public class AddDrinkActivityCommand : IRequest
    {
        public AddDrinkActivityCommand(string userId, string drinkId, DrinkType drinkType, string drinkName, double alcPrc, double volume)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            DrinkId = drinkId ?? throw new ArgumentNullException(nameof(drinkId));
            DrinkType = drinkType;
            DrinkName = drinkName ?? throw new ArgumentNullException(nameof(drinkName));
            AlcPrc = alcPrc;
            Volume = volume;
        }

        public string UserId { get; }
        public string DrinkId { get; }
        public DrinkType DrinkType { get; }
        public string DrinkName { get; }
        public double AlcPrc { get; }
        public double Volume { get; }

        public override string ToString()
        {
            return $"{{{nameof(UserId)}={UserId}, {nameof(DrinkId)}={DrinkId}, {nameof(DrinkType)}={DrinkType}, {nameof(DrinkName)}={DrinkName}, {nameof(AlcPrc)}={AlcPrc}, {nameof(Volume)}={Volume}}}";
        }
    }
}
