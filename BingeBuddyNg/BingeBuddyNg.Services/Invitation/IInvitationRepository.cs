using System;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Invitation.Persistence;

namespace BingeBuddyNg.Core.Invitation
{
    public interface IInvitationRepository
    {
        Task<InvitationTableEntity> GetAsync(Guid invitationToken);

        Task CreateAsync(Guid invitationToken, string userId);
    }
}
