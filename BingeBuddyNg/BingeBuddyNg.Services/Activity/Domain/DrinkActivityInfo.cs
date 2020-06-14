using System;
using BingeBuddyNg.Services.Drink;

namespace BingeBuddyNg.Services.Activity.Domain
{
    public class DrinkActivityInfo
    {
        public DrinkType DrinkType { get; private set; }
        public string DrinkId { get; private set; }
        public string DrinkName { get; private set; }
        public double? DrinkAlcPrc { get; private set; }
        public double? DrinkVolume { get; private set; }

        public DrinkActivityInfo(DrinkType drinkType, string drinkId, string drinkName, double? drinkAlcPrc, double? drinkVolume)
        {
            this.DrinkType = drinkType;
            this.DrinkId = drinkId ?? throw new ArgumentNullException(nameof(drinkId));
            this.DrinkName = drinkName ?? throw new ArgumentNullException(nameof(drinkName));
            this.DrinkAlcPrc = drinkAlcPrc;
            this.DrinkVolume = drinkVolume;
        }
    }
}
