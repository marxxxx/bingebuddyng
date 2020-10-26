namespace BingeBuddyNg.Core.User.DTO
{
    public class UpdateUserResponseDTO
    {
        public bool IsWeightMissing { get; }

        public bool IsGenderMissing { get; }

        public UpdateUserResponseDTO(bool isWeightMissing, bool isGenderMissing)
        {
            this.IsWeightMissing = isWeightMissing;
            this.IsGenderMissing = isGenderMissing;
        }
    }
}