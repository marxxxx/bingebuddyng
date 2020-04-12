using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace BingeBuddyNg.Services.Infrastructure
{
    public static class ConversionExtensions
    {
        public static TableContinuationToken ToContinuationToken(this string continuationToken)
        {
            return JsonConvert.DeserializeObject<TableContinuationToken>(continuationToken);
        }
    }
}
