using BingeBuddyNg.Core.Venue.DTO;
using BingeBuddyNg.Core.Venue.Persistence;

namespace BingeBuddyNg.Core.Venue
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

        public static VenueDTO ToDto(this VenueEntity ventity)
        {
            return new VenueDTO()
            {
                Id = ventity.Id,
                Location = ventity.Location,
                Name = ventity.Name,
                Distance = ventity.Distance
            };
        }

        public static VenueDTO ToDto(this Venue venue)
        {
            return new VenueDTO()
            {
                Id = venue.Id,
                Location = venue.Location,
                Name = venue.Name,
                Distance = venue.Distance
            };
        }

        public static Venue ToDomain(this VenueDTO dto)
        {
            return new Venue(id: dto.Id, location: dto.Location, name: dto.Name, distance: dto.Distance);
        }
    }
}
