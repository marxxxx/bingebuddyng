using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BingeBuddyNg.Services.Entitys
{
    public class JsonTableEntity<T> : TableEntity
    {
        private const string JsonEntityColumnName = "Json";

        public T Entity { get; set; }

        public JsonTableEntity()
        {

        }

        public JsonTableEntity(string partitionKey, string rowKey, T entity) : base(partitionKey, rowKey)
        {
            this.Entity = entity;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);

            if (properties.TryGetValue(JsonEntityColumnName, out EntityProperty value))
            {
                if (string.IsNullOrEmpty(value.StringValue) == false)
                {
                    this.Entity = JsonConvert.DeserializeObject<T>(value.StringValue);
                }
            }
        }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var values = base.WriteEntity(operationContext);
            if (values == null)
            {
                values = new Dictionary<string, EntityProperty>();
            }

            if (Entity != null)
            {
                values.Add(JsonEntityColumnName, new EntityProperty(JsonConvert.SerializeObject(this.Entity)));
            }

            return values;
        }

    }
}
