using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Interfaces
{
    public interface IInvitationRepository
    {
        Task<Invitation> GetInvitationAsync(string invitationToken);

        Task<string> CreateInvitationAsync(string userId);

        Task AcceptInvitationAsync(string userId, string token);
    }
}
