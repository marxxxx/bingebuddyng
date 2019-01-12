using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
namespace BingeBuddyNg.Services.Models.Generated
{
	public class Location
	{
		[JsonProperty("address")]
		public string Address { get; set; }
		[JsonProperty("crossStreet")]
		public string CrossStreet { get; set; }
		[JsonProperty("lat")]
		public double Lat { get; set; }
		[JsonProperty("lng")]
		public double Lng { get; set; }
		[JsonProperty("labeledLatLngs")]
		public List<LabeledLatLng> LabeledLatLngs { get; set; }
		[JsonProperty("distance")]
		public int Distance { get; set; }
		[JsonProperty("postalCode")]
		public string PostalCode { get; set; }
		[JsonProperty("cc")]
		public string Cc { get; set; }
		[JsonProperty("city")]
		public string City { get; set; }
		[JsonProperty("state")]
		public string State { get; set; }
		[JsonProperty("country")]
		public string Country { get; set; }
		[JsonProperty("formattedAddress")]
		public List<string> FormattedAddress { get; set; }
	}
	
}