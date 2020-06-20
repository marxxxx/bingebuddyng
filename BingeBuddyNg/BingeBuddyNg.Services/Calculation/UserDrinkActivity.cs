using System.Collections.Generic;
using System.Linq;
using BingeBuddyNg.Core.User;

namespace BingeBuddyNg.Core.Calculation
{
    public class UserDrinkActivity
    {
        public UserDrinkActivity(string userId, Gender gender, int weight, IEnumerable<DrinkActivityItem> drinks)
        {
            this.UserId = userId;
            this.Gender = gender;
            this.Weight = weight;
            this.Drinks = drinks != null ? drinks.ToList() : new List<DrinkActivityItem>();
        }

        public string UserId { get;set; }
        public Gender Gender { get; set; }
        public int Weight { get; set; }

        public List<DrinkActivityItem> Drinks { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(UserId)}={UserId}, {nameof(Gender)}={Gender}, {nameof(Weight)}={Weight}, {nameof(Drinks)}={Drinks}}}";
        }
    }
}
