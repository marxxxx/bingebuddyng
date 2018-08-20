using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.DTO
{
    public class UpdateUserResponseDTO
    {
        public bool IsWeightMissing { get; set; }
        public bool IsGenderMissing { get; set; }

        public UpdateUserResponseDTO()
        {

        }

        public UpdateUserResponseDTO(bool isWeightMissing, bool isGenderMissing)
        {
            this.IsWeightMissing = isWeightMissing;
            this.IsGenderMissing = isGenderMissing;
        }
    }
}
