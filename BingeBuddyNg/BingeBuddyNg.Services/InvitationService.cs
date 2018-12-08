using BingeBuddyNg.Services.Exceptions;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services
{
    public class InvitationService : IInvitationService
    {

        public IInvitationRepository InvitationRepository { get; }
        public IUserRepository UserRepository { get; }


        public InvitationService(IInvitationRepository invitationRepository, IUserRepository userRepository)
        {
            InvitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<InvitationInfo> GetInvitationInfoAsync(string invitationToken)
        {
            var invitation = await this.InvitationRepository.GetInvitationAsync(invitationToken);
            var user = await this.UserRepository.FindUserAsync(invitation.InvitingUserId);
            if(user == null)
            {
                throw new NotFoundException($"Inviting user {invitation.InvitingUserId} not found!");
            }

            var result = new InvitationInfo(invitationToken, user.ToUserInfo());
            return result;
        }
    }
}
