using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Drink
{
    public class DrinkEntity : TableEntity
    {
        public string UserId { get; set; }
        public string Id { get; set; }
        public DrinkType DrinkType { get; set; }
        public string Name { get; set; }
        public double? AlcPrc { get; set; }
        public double? Volume { get; set; }

        public DrinkEntity()
        {

        }
        public DrinkEntity(string userId, Drink drink) : base(userId, drink.Id)
        {
            if(drink == null)
            {
                throw new ArgumentNullException(nameof(drink));
            }

            this.UserId = userId;
            this.Id = drink.Id;
            this.Name = drink.Name;
            this.DrinkType = drink.DrinkType;
            this.AlcPrc = drink.AlcPrc;
            this.Volume = drink.Volume;

        }

        public Drink ToDrink()
        {
            return new Drink(Id, DrinkType, Name, AlcPrc, Volume);
        }
    }
}
