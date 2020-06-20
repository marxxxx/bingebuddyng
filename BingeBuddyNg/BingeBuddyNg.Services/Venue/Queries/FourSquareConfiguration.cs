using System;

namespace BingeBuddyNg.Core.Venue.Queries
{
    public class FourSquareConfiguration
    {
        public FourSquareConfiguration(string fourSquareApiClientKey, string fourSquareApiClientSecret)
        {
            FourSquareApiClientKey = fourSquareApiClientKey ?? throw new ArgumentNullException(nameof(fourSquareApiClientKey));
            FourSquareApiClientSecret = fourSquareApiClientSecret ?? throw new ArgumentNullException(nameof(fourSquareApiClientSecret));
        }

        public string FourSquareApiClientKey { get; }
        public string FourSquareApiClientSecret { get; }
    }
}
