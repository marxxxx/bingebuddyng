using Newtonsoft.Json;

namespace BingeBuddyNg.Core.Infrastructure.FourSquare
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