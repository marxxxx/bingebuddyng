using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? Weight { get; set; }
        public Gender Gender { get; set; }
        public string ProfileImageUrl { get; set; }
        public PushInfo PushInfo { get; set; }

        public User()
        {

        }

        public User(string id, string name, string profileImageUrl, Gender gender=Gender.Unknown, int? weight=null)
        {
            this.Id = !string.IsNullOrEmpty(id) ? id : throw new ArgumentNullException(nameof(id));
            this.Name = name;
            this.ProfileImageUrl = profileImageUrl;
            this.Weight = weight;
            this.Gender = gender;
        }
    }
}
