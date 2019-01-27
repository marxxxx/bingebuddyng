using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Invitation
{
    public interface IInvitationRepository
    {
        Task<Invitation> GetInvitationAsync(string invitationToken);

        Task<string> CreateInvitationAsync(string userId);

        Task<Invitation> AcceptInvitationAsync(string userId, string token);
    }
}
