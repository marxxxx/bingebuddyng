using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Infrastructure.EventGrid
{
    public interface IEventGridService
    {
        Task PublishAsync(string type, object eventData);
    }
}
