namespace BingeBuddyNg.Core.Calculation
{
    public class DrinkCalculationResult
    {
        public DrinkCalculationResult(string userId, int currentNightDrinks)
        {
            this.UserId = userId;
            this.CurrentNightDrinks = currentNightDrinks;
        }

        public DrinkCalculationResult(string userId, double currentAlcLevel, int currentNightDrinks)
        {
            UserId = userId;
            CurrentAlcLevel = currentAlcLevel;
            CurrentNightDrinks = currentNightDrinks;
        }

        public string UserId { get; }
        public double CurrentAlcLevel { get; }
        public int CurrentNightDrinks { get; }

        public override string ToString()
        {
            return $"{{{nameof(UserId)}={UserId}, {nameof(CurrentAlcLevel)}={CurrentAlcLevel}, {nameof(CurrentNightDrinks)}={CurrentNightDrinks}}}";
        }
    }
}