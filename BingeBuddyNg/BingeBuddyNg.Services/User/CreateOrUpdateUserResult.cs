namespace BingeBuddyNg.Core.User
{
    public class CreateOrUpdateUserResult
    {
        public bool IsNewUser { get; }
        public bool NameHasChanged { get; }
        public bool ProfilePicHasChanged { get; }
        public string OriginalUserName { get; set; }

        public CreateOrUpdateUserResult(bool isNewUser, bool profilePicHasChanged, bool nameHasChanged, string originalUserName)
        {
            this.IsNewUser = isNewUser;
            this.ProfilePicHasChanged = profilePicHasChanged;
            this.NameHasChanged = nameHasChanged;
            this.OriginalUserName = originalUserName;
        }
    }
}
