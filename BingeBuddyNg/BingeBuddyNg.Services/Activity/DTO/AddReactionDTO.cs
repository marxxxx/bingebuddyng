using BingeBuddyNg.Core.Activity.Domain;

namespace BingeBuddyNg.Services.Activity
{
    public class AddReactionDTO
    {
        public ReactionType Type { get; set; }
        public string Comment { get; set; }
    }
}
