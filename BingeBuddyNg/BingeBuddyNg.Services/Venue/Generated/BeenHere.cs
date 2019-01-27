using Newtonsoft.Json;

namespace BingeBuddyNg.Services.Venue.Generated
{
	public class BeenHere
	{
		[JsonProperty("count")]
		public int Count { get; set; }
		[JsonProperty("lastCheckinExpiredAt")]
		public int LastCheckinExpiredAt { get; set; }
		[JsonProperty("marked")]
		public bool Marked { get; set; }
		[JsonProperty("unconfirmedCount")]
		public int UnconfirmedCount { get; set; }
	}
	
}