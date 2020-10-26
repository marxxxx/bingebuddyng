using BingeBuddyNg.Core.Activity.Domain;

namespace BingeBuddyNg.Core.Activity
{
    public class ActivityFilterArgs
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