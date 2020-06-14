using System;

namespace BingeBuddyNg.Services.Activity.Domain
{
    public class MessageActivityInfo
    {
        public string Message { get; private set; }

        public MessageActivityInfo(string message)
        {
            this.Message = message ?? throw new ArgumentNullException(nameof(message));
        }        
    }
}
