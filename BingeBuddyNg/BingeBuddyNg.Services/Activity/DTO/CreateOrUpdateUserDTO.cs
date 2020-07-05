using BingeBuddyNg.Core.Infrastructure;

namespace BingeBuddyNg.Core.Activity.DTO
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
