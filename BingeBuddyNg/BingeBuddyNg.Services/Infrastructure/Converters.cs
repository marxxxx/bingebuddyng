using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace BingeBuddyNg.Core.Infrastructure
{
    public static class Converters
    {
        public static TableContinuationToken ToContinuationToken(this string continuationToken)
        {
            return JsonConvert.DeserializeObject<TableContinuationToken>(continuationToken);
        }
    }
}