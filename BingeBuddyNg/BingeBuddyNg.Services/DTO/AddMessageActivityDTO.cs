using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.DTO
{
    public class AddMessageActivityDTO
    {
        public string Message { get; set; }
        public Location Location { get; set; }
    }
}
