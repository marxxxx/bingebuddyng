using Newtonsoft.Json;

namespace BingeBuddyNg.Core.Infrastructure.FourSquare
{
    public class VenueRootObject
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("response")]
        public Response Response { get; set; }
    }
}