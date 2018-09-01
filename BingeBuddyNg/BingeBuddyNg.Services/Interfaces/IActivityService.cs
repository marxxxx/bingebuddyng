using BingeBuddyNg.Services.DTO;
using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
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

        Task AddReactionAsync(ReactionDTO reaction);
        Task AddImageActivityAsync(Stream stream, string fileName, Location location);
    }
}
