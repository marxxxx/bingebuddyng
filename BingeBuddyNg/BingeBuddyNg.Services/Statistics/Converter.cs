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
                ActivityCount = entity.ActivityCount != null ? int.Parse(entity.ActivityCount) : 0,
                AvgCount = entity.AvgCount != null ? double.Parse(entity.AvgCount, numFormat) : 0,
                MaxCount = entity.MaxCount != null ? int.Parse(entity.MaxCount) : 0,
                MinCount = entity.MinCount != null ? int.Parse(entity.MinCount) : 0,
                MedianActivityCount = entity.MedianActivityCount != null ? double.Parse(entity.MedianActivityCount, numFormat) : 0,
                MedianMaxAlcLevel = entity.MedianMaxAlcLevel != null ? double.Parse(entity.MedianMaxAlcLevel, numFormat) : 0,
                Percentage = entity.Percentage != null ? double.Parse(entity.Percentage, numFormat) : 0,
                Probability = entity.Probability != null ? double.Parse(entity.Probability, numFormat) : 0
            };
        }
    }
}
