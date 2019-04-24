using BingeBuddyNg.Services.Drink;
using System.ComponentModel.DataAnnotations;

namespace BingeBuddyNg.Services.Activity
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
