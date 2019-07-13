using BingeBuddyNg.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Activity
{
    public class CreateOrUpdateUserDTO
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string ProfileImageUrl { get; set; }
        public PushInfo PushInfo { get; set; }
        public string Language { get; set; }
    }
}
