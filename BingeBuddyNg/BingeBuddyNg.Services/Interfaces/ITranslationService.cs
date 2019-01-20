using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Interfaces
{
    public interface ITranslationService
    {
        Task<string> GetTranslationAsync(string key, string language);
    }
}