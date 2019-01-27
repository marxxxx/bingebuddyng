using Newtonsoft.Json;

namespace BingeBuddyNg.Services.Venue.Generated
{
	public class LabeledLatLng
	{
		[JsonProperty("label")]
		public string Label { get; set; }
		[JsonProperty("lat")]
		public double Lat { get; set; }
		[JsonProperty("lng")]
		public double Lng { get; set; }
	}
	
}