using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
namespace BingeBuddyNg.Services.Models.Generated
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