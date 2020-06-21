using System.Collections.Generic;
using System.Threading.Tasks;

namespace BingeBuddyNg.Core.Infrastructure
{
    public interface IFourSquareService
    {
        Task<List<FourSquare.Venue>> SearchVenuesAsync(float latitude, float longitude);
    }
}