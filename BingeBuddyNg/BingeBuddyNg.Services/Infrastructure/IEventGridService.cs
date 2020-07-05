using System.Threading.Tasks;

namespace BingeBuddyNg.Core.Infrastructure
{
    public interface IEventGridService
    {
        Task PublishAsync(string type, object eventData);
    }
}
