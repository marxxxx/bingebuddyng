using System.Threading.Tasks;
using BingeBuddyNg.Core.Invitation.Persistence;

namespace BingeBuddyNg.Core.Invitation
{
    public interface IInvitationRepository
    {
        Task<InvitationTableEntity> GetAsync(string invitationToken);

        Task<string> CreateAsync(string userId);
    }
}
