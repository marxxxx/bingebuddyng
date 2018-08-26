using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Models
{
    public class UserStatistics
    {
        public UserStatistics(string userId)
        {
            this.UserId = userId;
        }

        public UserStatistics(string userId, double currentAlcoholization, int currentNightDrinks)
        {
            UserId = userId;
            CurrentAlcoholization = currentAlcoholization;
            CurrentNightDrinks = currentNightDrinks;
        }

        public string UserId { get; set; }
        public double CurrentAlcoholization { get; set; }
        public int CurrentNightDrinks { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(UserId)}={UserId}, {nameof(CurrentAlcoholization)}={CurrentAlcoholization}, {nameof(CurrentNightDrinks)}={CurrentNightDrinks}}}";
        }
    }
}
