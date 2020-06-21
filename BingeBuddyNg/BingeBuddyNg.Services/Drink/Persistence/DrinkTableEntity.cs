using System;
using System.Collections.Generic;
using BingeBuddyNg.Core.Drink.DTO;
using BingeBuddyNg.Services.Infrastructure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace BingeBuddyNg.Core.Drink.Persistence
{
    public class DrinkTableEntity : TableEntity
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public DrinkType DrinkType { get; set; }

        public string Name { get; set; }

        public double? AlcPrc { get; set; }

        public double? Volume { get; set; }

        public DrinkTableEntity()
        {

        }
        public DrinkTableEntity(string userId, DrinkDTO drink) : base(userId, drink.Id)
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

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);
            if (properties.TryGetValue(nameof(DrinkType), out EntityProperty activityTypeValue))
            {
                this.DrinkType= Util.SafeParseEnum<DrinkType>(activityTypeValue.StringValue, DrinkType.Unknown);
            }
        }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var properties = base.WriteEntity(operationContext);
            properties.Add(nameof(DrinkType), new EntityProperty(DrinkType.ToString()));
            return properties;
        }
    }
}
