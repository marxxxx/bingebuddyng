using BingeBuddyNg.Services.Drink;
using BingeBuddyNg.Services.Game.Queries;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Services.Venue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Activity
{
    public class ActivityDTO
    {
        public string Id { get; set; }
        public ActivityType ActivityType { get; set; }
        public DateTime Timestamp { get; set; }
        public Location Location { get; set; }
        public string LocationAddress { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DrinkType DrinkType { get; set; }
        public string DrinkId { get; set; }
        public string DrinkName { get; set; }
        public double? DrinkAlcPrc { get; set; }
        public double? DrinkVolume { get; set; }
        public int DrinkCount { get; set; }
        public double? AlcLevel { get; set; }
        public string ImageUrl { get; set; }
        public string CountryLongName { get; set; }
        public string CountryShortName { get; set; }
        public Venue.Venue Venue { get; set; }

        public GameDTO GameInfo { get; set; }

        public UserInfo RegistrationUser { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OriginalUserName { get; set; }

        public List<ReactionDTO> Likes { get; set; } 
        public List<ReactionDTO> Cheers { get; set; }
        public List<CommentReactionDTO> Comments { get; set; } 
    }
}
