using Newtonsoft.Json;

namespace BingeBuddyNg.Services.Venue.Generated
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