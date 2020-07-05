using System.ComponentModel.DataAnnotations;
using BingeBuddyNg.Core.Drink;

namespace BingeBuddyNg.Core.Activity.DTO
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
