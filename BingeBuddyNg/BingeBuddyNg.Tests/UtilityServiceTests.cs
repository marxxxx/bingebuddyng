using BingeBuddyNg.Services.Activity;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace BingeBuddyNg.Tests
{
    public class UtilityServiceTests
    {
        public void DeserializeInCamelCase()
        {
            var activity = new ActivityDTO();
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var str = JsonConvert.SerializeObject(activity, new JsonSerializerSettings() { ContractResolver = contractResolver } );
            Assert.Contains(str, "id");
        }
    }
}
