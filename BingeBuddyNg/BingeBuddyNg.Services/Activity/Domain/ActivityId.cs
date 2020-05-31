using System;

namespace BingeBuddyNg.Services.Activity.Domain
{
    public class ActivityId
    {
        private static readonly DateTime MaxTimestamp = new DateTime(2100, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public string Value { get; private set; }

        private ActivityId(string value)
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static ActivityId Create(DateTime timestampUtc, string userId)
        {
            long ticks = (MaxTimestamp - timestampUtc).Ticks;
            string partitionKey = GetPartitionKey(timestampUtc);

            return new ActivityId($"{partitionKey}|{ticks}|{userId}");
        }

        private static string GetPartitionKey(DateTime timestampUtc)
        {
            int year = MaxTimestamp.Year - timestampUtc.Year;
            int month = 12 - timestampUtc.Month;
            return string.Format("{0:D2}-{1:D2}", year, month);
        }
    }
}
