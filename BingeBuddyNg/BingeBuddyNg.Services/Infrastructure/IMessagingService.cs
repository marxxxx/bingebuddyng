using System.Threading.Tasks;

namespace BingeBuddyNg.Core.Infrastructure
{
    public interface IMessagingService
    {
        Task SendMessageAsync<T>(T message);
    }
}
