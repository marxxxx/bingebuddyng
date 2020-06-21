using System;
using System.Collections.Generic;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.Drink;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Core.Game.Persistence;
using BingeBuddyNg.Services.Venue.Persistence;

namespace BingeBuddyNg.Core.Activity.Persistence
{
    public class ActivityEntity
    {
        public string Id { get; set; }

        public ActivityType ActivityType { get; set; }

        public DateTime Timestamp { get; set; }

        public Location Location { get; set; }

        public string LocationAddress { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public int DrinkCount { get; set; }

        public double? AlcLevel { get; set; }

        public string CountryLongName { get; set; }

        public string CountryShortName { get; set; }

        public VenueEntity Venue { get; set; }

        public string Message { get; set; }
        
        public DrinkType DrinkType { get; set; }
        
        public string DrinkId { get; set; }
        
        public string DrinkName { get; set; }
        
        public double? DrinkAlcPrc { get; set; }
        
        public double? DrinkVolume { get; set; }
        
        public string ImageUrl { get; set; }

        public GameEntity GameInfo { get; set; }

        public UserInfo RegistrationUser { get; set; }

        public string OriginalUserName { get; set; }

        public List<Reaction> Likes { get; set; }

        public List<Reaction> Cheers { get; set; }

        public List<CommentReaction> Comments { get; set; }
    }
}
