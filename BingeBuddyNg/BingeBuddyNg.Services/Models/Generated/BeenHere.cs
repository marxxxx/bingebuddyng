using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
namespace BingeBuddyNg.Services.Models.Generated
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