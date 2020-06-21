using Newtonsoft.Json;

namespace BingeBuddyNg.Core.Infrastructure.FourSquare
{
	public class Stats
	{
		[JsonProperty("tipCount")]
		public int TipCount { get; set; }
		[JsonProperty("usersCount")]
		public int UsersCount { get; set; }
		[JsonProperty("checkinsCount")]
		public int CheckinsCount { get; set; }
		[JsonProperty("visitsCount")]
		public int VisitsCount { get; set; }
	}
	
}