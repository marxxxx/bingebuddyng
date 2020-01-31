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
            return new PersonalUsagePerWeekdayDTO()
            {
                WeekDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), entity.weekDay, true),
                ActivityCount = entity.ActivityCount,
                AvgCount = entity.AvgCount,
                MaxCount = entity.MaxCount,
                MinCount = entity.MinCount,
                MedialActivityCount = entity.MedialActivityCount,
                MedianMaxAlcLevel = entity.MedianMaxAlcLevel,
                Percentage = entity.Percentage
            };
        }
    }
}
