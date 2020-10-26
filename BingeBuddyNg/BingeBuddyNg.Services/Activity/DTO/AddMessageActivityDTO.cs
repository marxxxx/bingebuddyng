using System.ComponentModel.DataAnnotations;

namespace BingeBuddyNg.Core.Activity.DTO
{
    public class AddMessageActivityDTO : AddActivityBaseDTO
    {
        [Required]
        public string Message { get; set; }
    }
}