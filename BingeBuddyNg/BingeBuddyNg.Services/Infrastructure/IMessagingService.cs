using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Infrastructure.Messaging
{
    public interface IMessagingService
    {
        Task SendMessageAsync<T>(T message);
    }
}
