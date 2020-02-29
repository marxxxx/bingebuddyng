using System;

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

        internal static PersonalUsagePerWeekdayDTO ToDto(this PersonalUsagePerWeekdayTableEntity entity)
        {
            var numFormat = new System.Globalization.CultureInfo("en").NumberFormat;
            
            return new PersonalUsagePerWeekdayDTO()
            {
                WeekDay = entity.weekDay,
                ActivityCount = ParseInt(entity.ActivityCount),
                AvgCount = ParseDouble(entity.AvgCount),
                MaxCount = ParseInt(entity.MaxCount),
                MinCount = ParseInt(entity.MinCount),
                MedianActivityCount = ParseDouble(entity.MedianActivityCount),
                MedianMaxAlcLevel = ParseDouble(entity.MedianMaxAlcLevel),
                Percentage = ParseDouble(entity.Percentage),
                Probability = ParseDouble(entity.Probability)
            };
        }

        private static int ParseInt(string value)
        {
            int.TryParse(value, out int result);
            return result;
        }

        private static double ParseDouble(string value)
        {
            double.TryParse(value, out double result);            
            return result;
        }
    }
}
