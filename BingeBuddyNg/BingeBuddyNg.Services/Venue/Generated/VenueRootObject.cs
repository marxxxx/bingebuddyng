using Newtonsoft.Json;

namespace BingeBuddyNg.Services.Venue.Generated
{
	public class VenueRootObject
	{
		[JsonProperty("meta")]
		public Meta Meta { get; set; }
		[JsonProperty("response")]
		public Response Response { get; set; }
	}
	
}