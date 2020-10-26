using System;

namespace BingeBuddyNg.Infrastructure
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