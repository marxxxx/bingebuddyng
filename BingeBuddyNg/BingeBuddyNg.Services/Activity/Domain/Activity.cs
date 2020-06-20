using System;
using System.Collections.Generic;
using System.Linq;

namespace BingeBuddyNg.Core.Activity.Domain
{
    public partial class Activity
    {
        public string Id { get; private set; }

        public ActivityType ActivityType { get; private set; }

        public DateTime Timestamp { get; private set; }

        public Location Location { get; private set; }

        public string LocationAddress { get; private set; }

        public string UserId { get; private set; }

        public string UserName { get; private set; }

        public int DrinkCount { get; private set; }

        public double? AlcLevel { get; private set; }

        public string CountryLongName { get; set; }

        public string CountryShortName { get; private set; }

        public Venue.Venue Venue { get; private set; }

        private List<Reaction> _likes = new List<Reaction>();
        public IReadOnlyCollection<Reaction> Likes => this._likes.AsReadOnly();

        private List<Reaction> _cheers = new List<Reaction>();
        public IReadOnlyCollection<Reaction> Cheers => this._cheers.AsReadOnly();

        private List<CommentReaction> _comments = new List<CommentReaction>();
        public IReadOnlyCollection<CommentReaction> Comments => this._comments.AsReadOnly();

        public DrinkActivityInfo Drink { get; private set; }

        public GameResultActivityInfo Game { get; private set; }

        public ImageActivityInfo Image { get; private set; }

        public MessageActivityInfo Message { get; private set; }

        public NotificationActivityInfo Notification { get; private set; }

        public RegistrationActivityInfo Registration { get; private set; }

        public RenameActivityInfo Rename { get; private set; }

        private Activity(
            string id,
            ActivityType type,
            DateTime timestamp,
            Location location,
            string userId,
            string userName)
        {
            this.Id = id;
            this.ActivityType = type;
            this.Timestamp = timestamp;
            this.Location = location;
            this.UserId = userId;
            this.UserName = userName;
        }

        public void AddComment(CommentReaction comment)
        {
            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment));
            }

            this._comments.Add(comment);
        }

        public void AddLike(Reaction reaction)
        {
            if(this.Likes.Any(l=>l.UserId == reaction.UserId))
            {
                throw new InvalidOperationException("User already liked this activity");
            }

            this._likes.Add(reaction);
        }

        public void AddCheers(Reaction reaction)
        {
            if (this.Cheers.Any(c => c.UserId == reaction.UserId))
            {
                throw new InvalidOperationException("User already cheered to this activity");
            }

            this._cheers.Add(reaction);
        }

        public void UpdateStats(int drinkCount, double alcLevel)
        {
            this.DrinkCount = drinkCount;
            this.AlcLevel = alcLevel;
        }

        public void UpdateLocation(string locationAddress, string countryShortName, string countryLongName)
        {
            this.LocationAddress = locationAddress;
            this.CountryShortName = countryShortName;
            this.CountryLongName = countryLongName;
        }
    }
}
