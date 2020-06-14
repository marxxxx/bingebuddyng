using System;

namespace BingeBuddyNg.Services.Drink
{
    public class Drink
    {
        public Drink()
        {
        }

        public Drink(string id, DrinkType drinkType, string name, double? alcPrc, double? volume)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            DrinkType = drinkType;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            AlcPrc = alcPrc ?? throw new ArgumentNullException(nameof(alcPrc));
            Volume = volume ?? throw new ArgumentNullException(nameof(volume));
        }

        public string Id { get; set; }
        public DrinkType DrinkType { get; set; }
        public string Name { get; set; }
        public double? AlcPrc { get; set; }
        public double? Volume { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Id)}={Id}, {nameof(DrinkType)}={DrinkType}, {nameof(Name)}={Name}, {nameof(AlcPrc)}={AlcPrc}, {nameof(Volume)}={Volume}}}";
        }
    }
}
