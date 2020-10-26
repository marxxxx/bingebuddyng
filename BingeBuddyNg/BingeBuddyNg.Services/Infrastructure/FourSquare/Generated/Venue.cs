using System.Collections.Generic;
using Newtonsoft.Json;

namespace BingeBuddyNg.Core.Infrastructure.FourSquare
{
    public class Venue
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("contact")]
        public Dictionary<string, string> Contact { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("categories")]
        public List<Category> Categories { get; set; }

        [JsonProperty("verified")]
        public bool Verified { get; set; }

        [JsonProperty("stats")]
        public Stats Stats { get; set; }

        [JsonProperty("beenHere")]
        public BeenHere BeenHere { get; set; }

        [JsonProperty("hereNow")]
        public HereNow HereNow { get; set; }

        [JsonProperty("referralId")]
        public string ReferralId { get; set; }

        [JsonProperty("venueChains")]
        public List<object> VenueChains { get; set; }

        [JsonProperty("hasPerk")]
        public bool HasPerk { get; set; }
    }
}