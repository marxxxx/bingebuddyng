using System.Threading.Tasks;
using BingeBuddyNg.Services.Invitation;

namespace BingeBuddyNg.Core.Invitation
{
    public interface IInvitationRepository
    {
        Task<InvitationTableEntity> GetAsync(string invitationToken);

        Task<string> CreateAsync(string userId);
    }
}
