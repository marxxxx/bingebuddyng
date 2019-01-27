using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Invitation
{
    public interface IInvitationService
    {
        Task<InvitationInfo> GetInvitationInfoAsync(string invitationToken);

        Task AcceptInvitationAsync(string userId, string invitationToken);
    }
}
