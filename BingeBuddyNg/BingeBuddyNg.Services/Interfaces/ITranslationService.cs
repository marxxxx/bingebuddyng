using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Interfaces
{
    public interface ITranslationService
    {
        Task<string> GetTranslationAsync(string language, string key, params object[] values);
    }
}