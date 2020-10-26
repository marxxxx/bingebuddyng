using System.Collections.Generic;
using Newtonsoft.Json;

namespace BingeBuddyNg.Core.Infrastructure.FourSquare
{
    public class Response
    {
        [JsonProperty("venues")]
        public List<Venue> Venues { get; set; }

        [JsonProperty("confident")]
        public bool Confident { get; set; }
    }
}