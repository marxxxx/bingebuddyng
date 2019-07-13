using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BingeBuddyNg.Services.Activity.Commands
{
    public class AddImageActivityCommand : IRequest
    {
        public AddImageActivityCommand(string userId, Stream stream, string fileName, double? lat, double? lng)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));

            if (lat.GetValueOrDefault() + lng.GetValueOrDefault() > 0)
            {
                Location = new Location(lat.Value, lng.Value);
            }
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
