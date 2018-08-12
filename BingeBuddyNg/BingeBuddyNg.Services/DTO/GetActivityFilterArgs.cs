using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.DTO
{
    public class GetActivityFilterArgs
    {
        public bool OnlyWithLocation { get; set; }

        public GetActivityFilterArgs()
        { }

        public GetActivityFilterArgs(bool onlyWithLocation)
        {
            this.OnlyWithLocation = onlyWithLocation;
        }

        public override string ToString()
        {
            return $"{nameof(OnlyWithLocation)} : {OnlyWithLocation}";
        }
    }
}
