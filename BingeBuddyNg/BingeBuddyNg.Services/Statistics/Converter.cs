using System;

namespace BingeBuddyNg.Services.Statistics
{
    public static class Converter
    {
        private static readonly IFormatProvider numFormat = new System.Globalization.CultureInfo("en").NumberFormat;

        public static UserStatisticsDTO ToDto(this UserStatistics stats)
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

        public static PersonalUsagePerWeekdayDTO ToDto(this PersonalUsagePerWeekdayTableEntity entity)
        {            
            return new PersonalUsagePerWeekdayDTO()
            {
                WeekDay = entity.WeekDay,
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
            int.TryParse(value, System.Globalization.NumberStyles.Integer, numFormat, out int result);
            return result;
        }

        private static double ParseDouble(string value)
        {
            double.TryParse(value, System.Globalization.NumberStyles.Float, numFormat, out double result);            
            return result;
        }
    }
}
