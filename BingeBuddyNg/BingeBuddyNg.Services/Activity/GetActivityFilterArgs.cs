﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace BingeBuddyNg.Services.Activity
{
    public class GetActivityFilterArgs
    {
        private const int DefaultActivityPageSize = 30;

        public ActivityFilterOptions FilterOptions { get; set; }
        public int PageSize { get; set; }
        public TableContinuationToken ContinuationToken { get; set; }
        public List<string> UserIds { get; set; }
        public ActivityType ActivityType { get; set; }

        public GetActivityFilterArgs()
        { }

        public GetActivityFilterArgs(int pageSize, TableContinuationToken continuationToken, ActivityFilterOptions filterOptions)
        {
            this.PageSize = pageSize;
            this.ContinuationToken = continuationToken;
            this.FilterOptions = filterOptions;
        }

        public GetActivityFilterArgs(ActivityFilterOptions filterOptions, 
            int pageSize = DefaultActivityPageSize,
            ActivityType activityType = ActivityType.None,
            string continuationToken = null)
        {
            this.ActivityType = activityType;
            this.FilterOptions = filterOptions;
            this.PageSize = pageSize;
            SetContinuationToken(continuationToken);
        }

        public GetActivityFilterArgs(IEnumerable<string> userIds, TableContinuationToken continuationToken) 
            : this(DefaultActivityPageSize, continuationToken, ActivityFilterOptions.None)
        {
            this.UserIds = userIds != null ? userIds.ToList() : null;
        }

        public void SetContinuationToken(string continuationToken)
        {
            if (continuationToken != null)
            {
                this.ContinuationToken = JsonConvert.DeserializeObject<TableContinuationToken>(continuationToken);
            }
        }

        public override string ToString()
        {
            return $"{{{nameof(FilterOptions)}={FilterOptions}, {nameof(PageSize)}={PageSize}}}";
        }
    }
}