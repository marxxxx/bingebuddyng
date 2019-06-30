using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Activity
{
    public interface IActivityService
    {       
        Task AddVenueActivityAsync(AddVenueActivityDTO activity);
    }
}
