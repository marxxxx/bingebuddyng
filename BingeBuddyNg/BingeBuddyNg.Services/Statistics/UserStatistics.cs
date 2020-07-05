namespace BingeBuddyNg.Core.Statistics
{
    public class UserStatistics
    {
        public UserStatistics(string userId)
        {
            this.UserId = userId;
        }

        public UserStatistics(string userId, double currentAlcoholization, int currentNightDrinks, int? score = null, int? totalDrinksLastMonth = null)
        {
            this.UserId = userId;
            this.CurrentAlcoholization = currentAlcoholization;
            this.CurrentNightDrinks = currentNightDrinks;
            this.TotalDrinksLastMonth = totalDrinksLastMonth;
            this.Score = score;
        }

        public string UserId { get; set; }
        public double CurrentAlcoholization { get; set; }
        public int CurrentNightDrinks { get; set; }
        public int? Score { get; set; }
        public int? TotalDrinksLastMonth { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(UserId)}={UserId}, {nameof(CurrentAlcoholization)}={CurrentAlcoholization}, {nameof(CurrentNightDrinks)}={CurrentNightDrinks}, {nameof(Score)}={Score}, {nameof(TotalDrinksLastMonth)}={TotalDrinksLastMonth}}}";
        }
    }
}
