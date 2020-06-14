using System;
using System.Collections.Generic;
using System.Linq;
using BingeBuddyNg.Services.Activity.Domain;
using BingeBuddyNg.Services.Activity.Domain.Events;
using BingeBuddyNg.Services.Drink;
using BingeBuddyNg.Services.Game.Persistence;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.Venue;

namespace BingeBuddyNg.Services.Activity
{
    public class Activity
    {
        private List<IDomainEvent> _domainEvents = new List<IDomainEvent>();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => this._domainEvents.AsReadOnly();

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

        public static Activity CreateDrinkActivity(
            string id,
            DateTime activityTimestamp,
            Location location,
            string userId,
            string userName,
            DrinkType drinkType,
            string drinkId,
            string drinkName,
            double drinkAlcPrc,
            double drinkVolume,
            Venue.Venue venue)
        {
            var activity = new Activity(id, ActivityType.Drink, activityTimestamp, location, userId, userName)
            {
                Drink = new DrinkActivityInfo(drinkType, drinkId, drinkName, drinkAlcPrc, drinkVolume),
                Venue = venue
            };

            return activity;
        }

        public static Activity CreateGameActivity(string id, DateTime timestamp, GameEntity game, UserInfo winner)
        {
            var activity = new Activity(id, ActivityType.GameResult, timestamp, Location.Nowhere, winner.UserId, winner.UserName)
            {
                Game = new GameResultActivityInfo(game)
            };

            return activity;
        }

        public static Activity CreateImageActivity(string id, DateTime activityTimestamp,
                    Location location, string userId, string userName, string imageUrl)
        {
            var activity = new Activity(id, ActivityType.Image, activityTimestamp,
                location, userId, userName)
            {
                Image = new ImageActivityInfo(imageUrl)
            };

            return activity;
        }

        public static Activity CreateMessageActivity(
            string id,
            DateTime timestamp,
            Location location,
            string userId,
            string userName,
            string message,
            Venue.Venue venue)
        {
            var activity = new Activity(id, ActivityType.Message, timestamp, location, userId, userName)
            {
                Message = new MessageActivityInfo(message),
                Venue = venue
            };

            return activity;
        }

        public static Activity CreateNotificationActivity(
            string id,
            DateTime timestamp,
            string userId,
            string userName,
            string message)
        {
            var activity = new Activity(id, ActivityType.Notification, timestamp, Location.Nowhere, userId, userName)
            {
                Notification = new NotificationActivityInfo(message)
            };

            return activity;
        }

        public static Activity CreateProfileImageUpdateActivity(string id, DateTime timestamp, string userId, string userName)
        {
            var activity = new Activity(
                id,
                ActivityType.ProfileImageUpdate,
                timestamp,
                Location.Nowhere,
                userId,
                userName);

            return activity;
        }

        public static Activity CreateRegistrationActivity(string id, DateTime timestamp, string userId, string userName, UserInfo registrationUser)
        {
            var activity = new Activity(id, ActivityType.Registration, timestamp, Location.Nowhere, userId, userName)
            {
                Registration = new RegistrationActivityInfo(registrationUser)
            };

            return activity;
        }

        public static Activity CreateRenameActivity(string id, DateTime timestamp, string userId, string userName, string originalUserName)
        {
            var activity = new Activity(id, ActivityType.Rename, timestamp, Location.Nowhere, userId, userName)
            {
                Rename = new RenameActivityInfo(originalUserName)
            };

            return activity;
        }

        public static Activity CreateVenueActivity(string id, DateTime activityTimestamp, string userId, string userName, Venue.Venue venue, VenueAction action)
        {
            if (action == VenueAction.Unknown)
            {
                throw new ArgumentException("Invalid venue action!");
            }

            var activity = new Activity(id, action == VenueAction.Enter ? ActivityType.VenueEntered : ActivityType.VenueLeft,
                activityTimestamp, venue.Location, userId, userName)
            {
                Venue = venue
            };

            return activity;
        }

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

            this._domainEvents.Add(new ReactionAdded(this.Id, ReactionType.Comment, comment.UserId, comment.Comment));
        }

        public void AddLike(Reaction reaction)
        {
            if(this.Likes.Any(l=>l.UserId == reaction.UserId))
            {
                throw new InvalidOperationException("User already liked this activity");
            }

            this._likes.Add(reaction);

            this._domainEvents.Add(new ReactionAdded(this.Id, ReactionType.Like, reaction.UserId));
        }

        public void AddCheers(Reaction reaction)
        {
            if (this.Cheers.Any(c => c.UserId == reaction.UserId))
            {
                throw new InvalidOperationException("User already cheered to this activity");
            }

            this._cheers.Add(reaction);

            this._domainEvents.Add(new ReactionAdded(this.Id, ReactionType.Cheers, reaction.UserId));
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
