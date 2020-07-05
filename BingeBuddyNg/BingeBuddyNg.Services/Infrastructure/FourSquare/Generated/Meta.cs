using Newtonsoft.Json;

namespace BingeBuddyNg.Core.Infrastructure.FourSquare
{
	public class Meta
	{
		[JsonProperty("code")]
		public int Code { get; set; }
		[JsonProperty("requestId")]
		public string RequestId { get; set; }
	}
	
}