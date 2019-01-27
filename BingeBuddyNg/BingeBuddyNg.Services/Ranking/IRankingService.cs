using System.Collections.Generic;
using System.Threading.Tasks;
using BingeBuddyNg.Services.User;

namespace BingeBuddyNg.Services.Ranking
{
    public interface IRankingService
    {
        Task<List<UserRanking>> GetDrinksRankingAsync();
        Task<List<UserRanking>> GetScoreRankingAsync();
    }
}
