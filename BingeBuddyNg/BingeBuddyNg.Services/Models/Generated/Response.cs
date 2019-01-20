using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
namespace BingeBuddyNg.Services.Models.Generated
{
	public class Response
	{
		[JsonProperty("venues")]
		public List<Venue> Venues { get; set; }
		[JsonProperty("confident")]
		public bool Confident { get; set; }
	}
	
}