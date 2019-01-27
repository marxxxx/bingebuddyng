using System;

namespace BingeBuddyNg.Services.Infrastructure
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) :base(message)
        {

        }
    }
}
