using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services
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
            this.ContinuationToken = JsonConvert.SerializeObject(continuationToken);
        }

        public List<T> ResultPage { get; set; }
        public string ContinuationToken { get; set; }
    }
}
