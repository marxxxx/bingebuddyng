using Newtonsoft.Json;

namespace BingeBuddyNg.Core.Infrastructure.FourSquare
{
	public class Icon
	{
		[JsonProperty("prefix")]
		public string Prefix { get; set; }
		[JsonProperty("suffix")]
		public string Suffix { get; set; }
	}
	
}