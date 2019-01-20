using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
namespace BingeBuddyNg.Services.Models.Generated
{
	public class LabeledLatLng
	{
		[JsonProperty("label")]
		public string Label { get; set; }
		[JsonProperty("lat")]
		public double Lat { get; set; }
		[JsonProperty("lng")]
		public double Lng { get; set; }
	}
	
}