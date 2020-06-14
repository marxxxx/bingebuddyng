namespace BingeBuddyNg.Services.Statistics
{
    public class UserStatisticsDTO
    {
        public string UserId { get; set; }
        public double CurrentAlcoholization { get; set; }
        public int CurrentNightDrinks { get; set; }
        public int? Score { get; set; }
        public int? TotalDrinksLastMonth { get; set; }
    }
}
