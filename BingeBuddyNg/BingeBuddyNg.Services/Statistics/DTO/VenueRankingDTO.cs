using System;

namespace BingeBuddyNg.Core.Statistics.DTO
{
    public class VenueRankingDTO
    {
        public VenueRankingDTO()
        {
        }

        public VenueRankingDTO(string venueId, string name, int count)
        {
            VenueId = venueId ?? throw new ArgumentNullException(nameof(venueId));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Count = count;
        }

        public string VenueId { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(VenueId)}={VenueId}, {nameof(Name)}={Name}, {nameof(Count)}={Count}}}";
        }
    }
}
