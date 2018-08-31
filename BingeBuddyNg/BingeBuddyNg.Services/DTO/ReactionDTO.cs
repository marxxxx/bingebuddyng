using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.DTO
{
    public class ReactionDTO
    {
        public ReactionType Type { get; set; }
        public string ActivityId { get; set; }
        public string Comment { get; set; }
    }
}
