using Microsoft.WindowsAzure.Storage.Table;

namespace BingeBuddyNg.Core.Venue.Persistence
{
    public class VenueUsersTableEntity : TableEntity
    {
        public string VenueId { get; set; }
        public string VenueName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }

        public VenueUsersTableEntity(string venueId, string venueName, string userId, string userName) : base(venueId, userId)
        {
            this.VenueId = venueId;
            this.VenueName = venueName;
            this.UserId = userId;
            this.UserName = userName;
        }

        public VenueUsersTableEntity()
        {
        }
    }
}