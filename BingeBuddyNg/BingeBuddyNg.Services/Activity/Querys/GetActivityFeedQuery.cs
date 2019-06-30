using BingeBuddyNg.Services.Infrastructure;
using MediatR;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;

namespace BingeBuddyNg.Services.Activity.Querys
{
    public class GetActivityFeedQuery : IRequest<PagedQueryResult<ActivityStatsDTO>>
    {
        public GetActivityFeedQuery(string userId, string continuationToken = null)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrEmpty(continuationToken) == false)
            {
                this.ContinuationToken = JsonConvert.DeserializeObject<TableContinuationToken>(continuationToken);
            }
        }

        public string UserId { get;}
        public TableContinuationToken ContinuationToken { get; }
    }
}
