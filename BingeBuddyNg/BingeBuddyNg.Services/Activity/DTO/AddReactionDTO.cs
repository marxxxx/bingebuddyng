using BingeBuddyNg.Core.Activity.Domain;

namespace BingeBuddyNg.Core.Activity.DTO
{
    public class AddReactionDTO
    {
        public ReactionType Type { get; set; }
        public string Comment { get; set; }
    }
}