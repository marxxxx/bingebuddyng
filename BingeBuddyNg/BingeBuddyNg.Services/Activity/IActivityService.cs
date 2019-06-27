using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Infrastructure;
using Microsoft.WindowsAzure.Storage.Table;

namespace BingeBuddyNg.Services.Activity
{
    public interface IActivityService
    {
        Task AddMessageActivityAsync(AddMessageActivityDTO activity);
        Task AddDrinkActivityAsync(AddDrinkActivityDTO activity);
        Task AddImageActivityAsync(Stream stream, string fileName, Location location);
        Task AddVenueActivityAsync(AddVenueActivityDTO activity);

        Task AddReactionAsync(ReactionDTO reaction);
    }
}
