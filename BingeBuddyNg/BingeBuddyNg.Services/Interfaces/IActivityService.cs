using BingeBuddyNg.Services.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Interfaces
{
    public interface IActivityService
    {
        Task<List<ActivityStatsDTO>> GetActivitiesAsync();
        Task<List<ActivityAggregationDTO>> GetDrinkActivityAggregationAsync();

        Task AddMessageActivityAsync(AddMessageActivityDTO activity);
        Task AddDrinkActivityAsync(AddDrinkActivityDTO request);
    }
}
