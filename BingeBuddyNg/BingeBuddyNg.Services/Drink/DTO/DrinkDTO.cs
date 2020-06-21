using System;

namespace BingeBuddyNg.Core.Drink.DTO
{
    public class DrinkDTO
    {
        public DrinkDTO()
        {
        }

        public DrinkDTO(string id, DrinkType drinkType, string name, double? alcPrc, double? volume)
        {
            this.Id = id ?? throw new ArgumentNullException(nameof(id));
            this.DrinkType = drinkType;
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.AlcPrc = alcPrc;
            this.Volume = volume;
        }

        public string Id { get; set; }

        public DrinkType DrinkType { get; set; }

        public string Name { get; set; }

        public double? AlcPrc { get; set; }

        public double? Volume { get; set; }
    }
}
