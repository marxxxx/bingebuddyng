using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace BingeBuddyNg.Services.Infrastructure
{
    public class PagedQueryResult<T>
    {
        public PagedQueryResult()
        {
            this.ResultPage = new List<T>();
        }

        public PagedQueryResult(List<T> resultPage, string continuationToken)
        {
            this.ResultPage = resultPage;
            this.ContinuationToken = continuationToken;

        }
        public PagedQueryResult(List<T> resultPage, TableContinuationToken continuationToken)
        {
            this.ResultPage = resultPage;
            this.ContinuationToken = continuationToken != null ? JsonConvert.SerializeObject(continuationToken) : null;
        }

        public List<T> ResultPage { get; set; }
        public string ContinuationToken { get; set; }
    }
}
