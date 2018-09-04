using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.DTO
{
    public class GetActivityFilterArgs
    {
        public bool OnlyWithLocation { get; set; }
        public int PageSize { get; set; }
        public TableContinuationToken ContinuationToken { get; set; }

        public GetActivityFilterArgs()
        { }

        public GetActivityFilterArgs(bool onlyWithLocation, int pageSize = 20, string continuationToken = null)
        {
            this.OnlyWithLocation = onlyWithLocation;
            this.PageSize = pageSize;
            if (continuationToken != null)
            {
                this.ContinuationToken = JsonConvert.DeserializeObject<TableContinuationToken>(continuationToken);
            }
        }

        public override string ToString()
        {
            return $"{{{nameof(OnlyWithLocation)}={OnlyWithLocation}, {nameof(PageSize)}={PageSize}}}";
        }
    }
}
