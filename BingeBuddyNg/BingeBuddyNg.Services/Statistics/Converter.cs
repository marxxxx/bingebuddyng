namespace BingeBuddyNg.Services.Statistics
{
    internal static class Converter
    {
        internal static UserStatisticsDTO ConvertUserStatisticsToDto(UserStatistics stats)
        {
            if (stats == null)
                return null;

            return new UserStatisticsDTO()
            {
                UserId = stats.UserId,
                Score = stats.Score,
                CurrentAlcoholization = stats.CurrentAlcoholization,
                CurrentNightDrinks = stats.CurrentNightDrinks,
                TotalDrinksLastMonth = stats.TotalDrinksLastMonth
            };
        }
    }
}
