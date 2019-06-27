using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BingeBuddyNg.Services.Activity.Commands
{
    public class AddImageActivityCommand : IRequest
    {
        public AddImageActivityCommand(Stream stream, string fileName, Location location)
        {
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            Location = location ?? throw new ArgumentNullException(nameof(location));
        }

        public string UserId { get; }
        public Stream Stream { get; }
        public string FileName { get; }
        public  Location Location { get; }

        public override string ToString()
        {
            return $"{{{nameof(Stream)}={Stream}, {nameof(FileName)}={FileName}, {nameof(Location)}={Location}}}";
        }
    }
}
