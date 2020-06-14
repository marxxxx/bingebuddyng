using System;

namespace BingeBuddyNg.Services.Activity.Domain
{
    public class RenameActivityInfo
    {
        public string OriginalUserName { get; }

        public RenameActivityInfo(string originalUserName)
        {
            this.OriginalUserName = originalUserName ?? throw new ArgumentNullException(nameof(originalUserName));
        }
    }
}
