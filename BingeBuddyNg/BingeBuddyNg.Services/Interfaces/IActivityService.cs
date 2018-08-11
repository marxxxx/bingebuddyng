using BingeBuddyNg.Services.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Interfaces
{
    public interface IActivityService
    {
        Task AddMessageActivityAsync(AddMessageActivityDTO activity);
        Task<List<ActivityAggregationDTO>> GetActivityAggregationAsync();
    }
}
