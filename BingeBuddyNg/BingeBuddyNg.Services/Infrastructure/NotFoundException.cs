using System;

namespace BingeBuddyNg.Core.Infrastructure
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) :base(message)
        {

        }
    }
}
