using System.Threading.Tasks;

namespace BingeBuddyNg.Core.Infrastructure
{
    public interface ITranslationService
    {
        Task<string> GetTranslationAsync(string language, string key, params object[] values);
    }
}