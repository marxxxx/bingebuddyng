using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Interfaces
{
    public interface IFriendRequestService
    {
        Task AcceptFriendRequestAsync(string acceptingUserId, string requestingUserId);
        Task AddFriendRequestAsync(string requestingUserId, string friendUserId);
        Task DeclineFriendRequestAsync(string decliningUserId, string requestingUserId);
    }
}