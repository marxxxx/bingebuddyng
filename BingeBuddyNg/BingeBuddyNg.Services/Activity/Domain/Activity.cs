using System;
using System.Collections.Generic;
using BingeBuddyNg.Services.Activity.Domain.Events;

namespace BingeBuddyNg.Services.Activity
{
    public abstract class Activity
    {
        public List<IDomainEvent> DomainEvents { get; } = new List<IDomainEvent>();

        public string Id { get; private set; }

        public ActivityType ActivityType { get; private set; }

        public DateTime Timestamp { get; private set; }

        public Location Location { get; private set; }

        public string LocationAddress { get; protected set; }

        public string UserId { get; private set; }

        public string UserName { get; private set; }

        public int DrinkCount { get; private set; }

        public double? AlcLevel { get; private set; }

        public string CountryLongName { get; set; }

        public string CountryShortName { get; private set; }

        public Venue.Venue Venue { get; protected set; }

        public List<Reaction> Likes { get; private set; } = new List<Reaction>();

        public List<Reaction> Cheers { get; private set; } = new List<Reaction>();

        public List<CommentReaction> Comments { get; private set; } = new List<CommentReaction>();

        protected Activity(
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

        public void AddComment(CommentReaction reaction)
        {
            this.Comments.Add(reaction);

            this.DomainEvents.Add(new ReactionAdded(this.Id, ReactionType.Comment, reaction.UserId, reaction.Comment));
        }

        public void AddLike(Reaction reaction)
        {
            this.Likes.Add(reaction);

            this.DomainEvents.Add(new ReactionAdded(this.Id, ReactionType.Like, reaction.UserId));
        }

        public void AddCheers(Reaction reaction)
        {
            this.Cheers.Add(reaction);

            this.DomainEvents.Add(new ReactionAdded(this.Id, ReactionType.Cheers, reaction.UserId));
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
