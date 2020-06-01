namespace BingeBuddyNg.Services.Activity
{
    public class GetActivityFilterArgs
    {
        private const int DefaultActivityPageSize = 30;

        public ActivityFilterOptions FilterOptions { get; set; }
        public int PageSize { get; set; } = DefaultActivityPageSize;
        public ActivityType ActivityType { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(FilterOptions)}={FilterOptions}, {nameof(PageSize)}={PageSize}}}";
        }
    }
}
