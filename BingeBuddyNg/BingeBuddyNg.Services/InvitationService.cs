using BingeBuddyNg.Services.Exceptions;
using BingeBuddyNg.Services.Interfaces;
using BingeBuddyNg.Services.Models;
using BingeBuddyNg.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services
{
    public class InvitationService : IInvitationService
    {
        private ILogger<InvitationService> logger;

        public IInvitationRepository InvitationRepository { get; }
        public IUserRepository UserRepository { get; }
        public INotificationService NotificationService { get; }
        public IActivityRepository ActivityRepository { get; }
        public ITranslationService TranslationService { get; }
        public IUserStatsRepository UserStatsRepository { get; }


        public InvitationService(IInvitationRepository invitationRepository, IUserRepository userRepository,
            IUserStatsRepository userStatsRepository,
            INotificationService notificationService, 
            IActivityRepository activityRepository,
            ITranslationService translationService,
            ILogger<InvitationService> logger)
        {
            this.InvitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));
            this.UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.UserStatsRepository = userStatsRepository ?? throw new ArgumentNullException(nameof(userStatsRepository));
            this.NotificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.TranslationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            if (userId != invitation.InvitingUserId)
            {                
                var invitingUser = await this.UserRepository.FindUserAsync(invitation.InvitingUserId);
                var acceptingUser = await this.UserRepository.FindUserAsync(invitation.AcceptingUserId);

                if (invitingUser != null && acceptingUser != null)
                {
                    await this.UserRepository.AddFriendAsync(invitingUser.Id, acceptingUser.Id);
                }

                try
                {
                    await this.UserStatsRepository.IncreaseScoreAsync(invitingUser.Id, Constants.Scores.FriendInvitation);

                    var message = await TranslationService.GetTranslationAsync(invitingUser.Language, "RecruitmentActivityMessage", acceptingUser.Name, Constants.Scores.FriendInvitation);
                    var notificationActivity = Activity.CreateNotificationActivity(DateTime.UtcNow, invitingUser.Id, invitingUser.Name, message);

                    await this.ActivityRepository.AddActivityAsync(notificationActivity);
                }
                catch(Exception ex)
                {
                    // log error
                    logger.LogError($"Error increasing score for user {invitingUser}: {ex}");
                }

                if (invitingUser != null && invitingUser.PushInfo != null)
                {
                    string userName = acceptingUser?.Name ?? await TranslationService.GetTranslationAsync(invitingUser.Language, "Somebody");
                    string messageContent = await TranslationService.GetTranslationAsync(invitingUser.Language, "AcceptedInvitation", userName);

                    var message = new NotificationMessage(Constants.Urls.ApplicationIconUrl, Constants.Urls.ApplicationIconUrl, Constants.Urls.ApplicationUrl,
                        "Binge Buddy", messageContent);
                    this.NotificationService.SendMessage(new[] { invitingUser.PushInfo }, message);
                }
            }
        }
    }
}
