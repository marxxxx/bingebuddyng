using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
namespace BingeBuddyNg.Services.Models.Generated
{
	public class Meta
	{
		[JsonProperty("code")]
		public int Code { get; set; }
		[JsonProperty("requestId")]
		public string RequestId { get; set; }
	}
	
}