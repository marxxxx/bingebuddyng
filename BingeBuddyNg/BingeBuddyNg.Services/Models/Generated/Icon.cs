using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
namespace BingeBuddyNg.Services.Models.Generated
{
	public class Icon
	{
		[JsonProperty("prefix")]
		public string Prefix { get; set; }
		[JsonProperty("suffix")]
		public string Suffix { get; set; }
	}
	
}