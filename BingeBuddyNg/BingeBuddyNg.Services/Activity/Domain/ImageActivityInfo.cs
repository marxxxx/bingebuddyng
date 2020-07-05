using System;

namespace BingeBuddyNg.Core.Activity.Domain
{
    public class ImageActivityInfo
    {
        public string ImageUrl { get; private set; }

        public ImageActivityInfo(string imageUrl)
        {
            this.ImageUrl = imageUrl ?? throw new ArgumentNullException(nameof(imageUrl));
        }
    }
}
