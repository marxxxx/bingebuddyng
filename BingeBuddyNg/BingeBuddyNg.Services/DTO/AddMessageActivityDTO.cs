using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BingeBuddyNg.Services.DTO
{
    public class AddMessageActivityDTO: AddActivityBaseDTO
    {
        [Required]
        public string Message { get; set; }        
    }
}
