using System.Threading.Tasks;

namespace BingeBuddyNg.Core.Invitation
{
    public interface IInvitationRepository
    {
        Task<Invitation> GetInvitationAsync(string invitationToken);

        Task<string> CreateInvitationAsync(string userId);

        Task<Invitation> AcceptInvitationAsync(string userId, string token);
    }
}
