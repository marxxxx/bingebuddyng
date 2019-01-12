using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
namespace BingeBuddyNg.Services.Models.Generated
{
	public class VenueRootObject
	{
		[JsonProperty("meta")]
		public Meta Meta { get; set; }
		[JsonProperty("response")]
		public Response Response { get; set; }
	}
	
}