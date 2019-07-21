using BingeBuddyNg.Services.Infrastructure;
using MediatR;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace BingeBuddyNg.Services.Activity.Querys
{
    [DebuggerStepThrough]
    public class GetActivityFeedQuery : IRequest<PagedQueryResult<ActivityStatsDTO>>
    {
        public GetActivityFeedQuery(string userId, string startActivityId = null, string continuationToken = null)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrEmpty(startActivityId) && startActivityId != "null")
            {
                this.StartActivityId = startActivityId;
            }
            if (string.IsNullOrEmpty(continuationToken) == false)
            {
                this.ContinuationToken = JsonConvert.DeserializeObject<TableContinuationToken>(continuationToken);
            }
        }

        public string UserId { get;}
        public string StartActivityId { get; }
        public TableContinuationToken ContinuationToken { get; }
    }
}
