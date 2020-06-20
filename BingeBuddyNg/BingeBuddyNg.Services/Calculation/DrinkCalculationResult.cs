namespace BingeBuddyNg.Core.Calculation
{
    public class DrinkCalculationResult
    {
        public DrinkCalculationResult(string userName, int currentNightDrinks)
        {
            this.UserName = userName;
            this.CurrentNightDrinks = currentNightDrinks;
        }

        public DrinkCalculationResult(string userName, double currentAlcLevel, int currentNightDrinks)
        {
            UserName = userName;
            CurrentAlcLevel = currentAlcLevel;
            CurrentNightDrinks = currentNightDrinks;
        }

        public string UserName { get; set; }
        public double CurrentAlcLevel { get; set; }
        public int CurrentNightDrinks { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(UserName)}={UserName}, {nameof(CurrentAlcLevel)}={CurrentAlcLevel}, {nameof(CurrentNightDrinks)}={CurrentNightDrinks}}}";
        }
    }
}
