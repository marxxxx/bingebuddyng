using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Shared;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Invitation.Commands
{
    public class InvitationCommandHandler :
        IRequestHandler<AcceptInvitationCommand>,
        IRequestHandler<CreateInvitationCommand, string>
    {
        private ILogger<InvitationCommandHandler> logger;

        public InvitationCommandHandler(IInvitationRepository invitationRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IActivityRepository activityRepository,
            ITranslationService translationService,
            IUserStatsRepository userStatsRepository,
            ILogger<InvitationCommandHandler> logger)
        {
            InvitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            NotificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            ActivityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            TranslationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
            UserStatsRepository = userStatsRepository ?? throw new ArgumentNullException(nameof(userStatsRepository));

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IInvitationRepository InvitationRepository { get; }
        public IUserRepository UserRepository { get; }
        public INotificationService NotificationService { get; }
        public IActivityRepository ActivityRepository { get; }
        public ITranslationService TranslationService { get; }
        public IUserStatsRepository UserStatsRepository { get; }

        public async Task<Unit> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
        {
            var invitation = await this.InvitationRepository.AcceptInvitationAsync(request.AcceptingUserId, request.InvitationToken);
            if (request.AcceptingUserId != invitation.InvitingUserId)
            {
                var invitingUser = await this.UserRepository.FindUserAsync(invitation.InvitingUserId);
                var acceptingUser = await this.UserRepository.FindUserAsync(request.AcceptingUserId);

                if (invitingUser != null && acceptingUser != null)
                {
                    await this.UserRepository.AddFriendAsync(invitingUser.Id, acceptingUser.Id);
                }

                try
                {
                    await this.UserStatsRepository.IncreaseScoreAsync(invitingUser.Id, Constants.Scores.FriendInvitation);

                    var message = await TranslationService.GetTranslationAsync(invitingUser.Language, "RecruitmentActivityMessage", acceptingUser.Name, Constants.Scores.FriendInvitation);
                    var notificationActivity = Activity.Activity.CreateNotificationActivity(DateTime.UtcNow, invitingUser.Id, invitingUser.Name, message);

                    await this.ActivityRepository.AddActivityAsync(notificationActivity);
                }
                catch (Exception ex)
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
                    this.NotificationService.SendWebPushMessage(new[] { invitingUser.PushInfo }, message);
                }
            }

            return Unit.Value;
        }

        public async Task<string> Handle(CreateInvitationCommand request, CancellationToken cancellationToken)
        {
            var invitationToken = await InvitationRepository.CreateInvitationAsync(request.UserId);
            return invitationToken;
        }
    }
}
