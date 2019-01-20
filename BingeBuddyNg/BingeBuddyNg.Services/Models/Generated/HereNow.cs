using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
namespace BingeBuddyNg.Services.Models.Generated
{
	public class HereNow
	{
		[JsonProperty("count")]
		public int Count { get; set; }
		[JsonProperty("summary")]
		public string Summary { get; set; }
		[JsonProperty("groups")]
		public List<object> Groups { get; set; }
	}
	
}