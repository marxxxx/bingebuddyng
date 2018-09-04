using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services
{
    public class PagedQueryResult<T>
    {
        public PagedQueryResult(List<T> resultPage, TableContinuationToken continuationToken)
        {
            this.ResultPage = resultPage;
            this.ContinuationToken = continuationToken;
        }

        public List<T> ResultPage { get; set; }
        public TableContinuationToken ContinuationToken { get; set; }
    }
}
