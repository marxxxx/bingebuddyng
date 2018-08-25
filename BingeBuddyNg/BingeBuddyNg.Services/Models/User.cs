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
        
    }
}
