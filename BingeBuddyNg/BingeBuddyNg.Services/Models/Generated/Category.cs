using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
namespace BingeBuddyNg.Services.Models.Generated
{
	public class Category
	{
		[JsonProperty("id")]
		public string Id { get; set; }
		[JsonProperty("name")]
		public string Name { get; set; }
		[JsonProperty("pluralName")]
		public string PluralName { get; set; }
		[JsonProperty("shortName")]
		public string ShortName { get; set; }
		[JsonProperty("icon")]
		public Icon Icon { get; set; }
		[JsonProperty("primary")]
		public bool Primary { get; set; }
	}
	
}