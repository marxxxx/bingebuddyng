using BingeBuddyNg.Services.Exceptions;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using BingeBuddyNg.Shared;
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
        public INotificationService NotificationService { get; }


        public InvitationService(IInvitationRepository invitationRepository, IUserRepository userRepository,
            INotificationService notificationService)
        {
            InvitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            NotificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public async Task<InvitationInfo> GetInvitationInfoAsync(string invitationToken)
        {
            var invitation = await this.InvitationRepository.GetInvitationAsync(invitationToken);
            var user = await this.UserRepository.FindUserAsync(invitation.InvitingUserId);
            if(user == null)
            {
                throw new NotFoundException($"Inviting user {invitation.InvitingUserId} not found!");
            }

            var result = new InvitationInfo(invitation, user.ToUserInfo());
            return result;
        }

        public async Task AcceptInvitationAsync(string userId, string invitationToken)
        {
            var invitation = await this.InvitationRepository.AcceptInvitationAsync(userId, invitationToken);

            var invitingUser = await this.UserRepository.FindUserAsync(invitation.InvitingUserId);
            var acceptingUser = await this.UserRepository.FindUserAsync(invitation.AcceptingUserId);

            if(invitingUser != null && acceptingUser != null)
            {
                await this.UserRepository.AddFriendAsync(invitingUser.Id, acceptingUser.Id);
            }

            if (invitingUser != null && invitingUser.PushInfo != null)
            {
                // TODO: Localize!
                string userName = acceptingUser?.Name ?? "jemand";
                
                var message = new NotificationMessage(Constants.Urls.ApplicationIconUrl, Constants.Urls.ApplicationIconUrl, Constants.Urls.ApplicationUrl, 
                    "Binge Buddy", $"{userName} hat deine Einladung angenommen.");
                this.NotificationService.SendMessage(new[] { invitingUser.PushInfo }, message);
            }
             

        }
    }
}
