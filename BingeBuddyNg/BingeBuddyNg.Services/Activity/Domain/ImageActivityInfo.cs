using System;

namespace BingeBuddyNg.Services.Activity.Domain
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
