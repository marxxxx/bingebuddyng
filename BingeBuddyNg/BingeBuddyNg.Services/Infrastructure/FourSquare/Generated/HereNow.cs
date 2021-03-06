﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace BingeBuddyNg.Core.Infrastructure.FourSquare
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