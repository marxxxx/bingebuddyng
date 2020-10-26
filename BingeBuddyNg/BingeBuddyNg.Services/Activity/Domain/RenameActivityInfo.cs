using System;

namespace BingeBuddyNg.Core.Activity.Domain
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