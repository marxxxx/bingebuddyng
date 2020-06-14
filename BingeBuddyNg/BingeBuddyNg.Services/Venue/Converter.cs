using BingeBuddyNg.Services.Venue.Persistence;

namespace BingeBuddyNg.Services.Venue
{
    public static class Converter
    {
        public static VenueEntity ToEntity(this Venue venue)
        {
            return new VenueEntity()
            {
                Id = venue.Id,
                Location = venue.Location,
                Name = venue.Name,
                Distance = venue.Distance
            };
        }

        public static Venue ToDomain(this VenueEntity entity)
        {
            return new Venue(entity.Id, entity.Location, entity.Name, entity.Distance);
        }
    }
}
