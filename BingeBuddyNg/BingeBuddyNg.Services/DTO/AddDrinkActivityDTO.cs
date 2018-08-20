using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BingeBuddyNg.Services.DTO
{
    public class AddDrinkActivityDTO : AddActivityBaseDTO
    {
        public string DrinkId { get; set; }
        [Required]
        public DrinkType DrinkType { get; set; }
        [Required]
        public string DrinkName { get; set; }
        [Required]
        public double AlcPrc { get; set; }
        [Required]
        public double Volume { get; set; }
    }
}
