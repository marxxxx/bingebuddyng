using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Venue.Querys
{
    public class SearchVenuesQuery : IRequest<List<VenueModel>>
    {
        public SearchVenuesQuery(float latitude, float longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public float Latitude { get; }
        public float Longitude { get; }
    }
}
