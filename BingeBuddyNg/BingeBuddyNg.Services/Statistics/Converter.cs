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
                ActivityCount = int.Parse(entity.ActivityCount),
                AvgCount = double.Parse(entity.AvgCount, numFormat),
                MaxCount = int.Parse(entity.MaxCount),
                MinCount = int.Parse(entity.MinCount),
                MedianActivityCount = double.Parse(entity.MedianActivityCount, numFormat),
                MedianMaxAlcLevel = double.Parse(entity.MedianMaxAlcLevel, numFormat),
                Percentage = double.Parse(entity.Percentage, numFormat)
            };
        }
    }
}
