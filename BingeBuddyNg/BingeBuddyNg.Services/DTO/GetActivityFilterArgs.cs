using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.DTO
{
    public class GetActivityFilterArgs
    {
        public bool OnlyWithLocation { get; set; }
        public int PageSize { get; set; }

        public GetActivityFilterArgs()
        { }

        public GetActivityFilterArgs(bool onlyWithLocation, int pageSize = 100)
        {
            this.OnlyWithLocation = onlyWithLocation;
            this.PageSize = pageSize;
        }

        public override string ToString()
        {
            return $"{{{nameof(OnlyWithLocation)}={OnlyWithLocation}, {nameof(PageSize)}={PageSize}}}";
        }
    }
}
