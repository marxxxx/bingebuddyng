using System;

namespace BingeBuddyNg.Core.Activity.Persistence
{
    public static class ActivityKeyFactory
    {
        private static readonly DateTime MaxTimestamp = new DateTime(2100, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static string CreatePartitionKey(DateTime timestampUtc)
        {
            int year = MaxTimestamp.Year - timestampUtc.Year;
            int month = 12 - timestampUtc.Month;
            return string.Format("{0:D2}-{1:D2}", year, month);
        }

        public static string CreateRowKey(DateTime timestampUtc, string userId)
        {
            long ticks = (MaxTimestamp - timestampUtc).Ticks;
            string partitionKey = CreatePartitionKey(timestampUtc);

            return $"{partitionKey}|{ticks}|{userId}";
        }

        public static string GetPartitionKeyFromRowKey(string rowKey)
        {
            string[] tokens = rowKey.Split('|');
            return tokens[0];
        }

        public static string CreatePerUserRowKey(DateTime timestampUtc)
        {
            return timestampUtc.ToString("yyyyMMddHHmmss");
        }        
    }
}
