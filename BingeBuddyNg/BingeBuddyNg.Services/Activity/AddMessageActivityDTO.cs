using System.ComponentModel.DataAnnotations;

namespace BingeBuddyNg.Services.Activity
{
    public class AddMessageActivityDTO: AddActivityBaseDTO
    {
        [Required]
        public string Message { get; set; }        
    }
}
