using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace BingeBuddyNg.Services.Activity
{
    public class GetActivityFilterArgs
    {
        private const int DefaultActivityPageSize = 30;

        public string UserId { get; set; }
        public ActivityFilterOptions FilterOptions { get; set; }
        public int PageSize { get; set; } = DefaultActivityPageSize;
        public TableContinuationToken ContinuationToken { get; set; }
        public ActivityType ActivityType { get; set; }
        public string StartActivityId { get; set; }

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
