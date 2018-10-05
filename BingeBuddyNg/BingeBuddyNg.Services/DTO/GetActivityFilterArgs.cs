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
        public IEnumerable<string> FilteredUserIds { get; set; }

        public GetActivityFilterArgs()
        { }

        public GetActivityFilterArgs(bool onlyWithLocation, IEnumerable<string> filteredUserIds, int pageSize, TableContinuationToken continuationToken)
        {
            this.FilteredUserIds = filteredUserIds;
            this.OnlyWithLocation = onlyWithLocation;
            this.PageSize = pageSize;
            this.ContinuationToken = continuationToken;
        }

        public GetActivityFilterArgs(bool onlyWithLocation, IEnumerable<string> filteredUserIds, int pageSize = 30, string continuationToken = null)
        {
            this.FilteredUserIds = filteredUserIds;
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
