using System;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.Ranking;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Shared;
using MediatR;
using Microsoft.Extensions.Logging;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.Invitation.Commands
{
    public class AcceptInvitationCommand : IRequest
    {
        public string AcceptingUserId { get; }

        public Guid InvitationToken { get; }

        public AcceptInvitationCommand(string acceptingUserId, Guid invitationToken)
        {
            AcceptingUserId = acceptingUserId ?? throw new ArgumentNullException(nameof(acceptingUserId));
            InvitationToken = invitationToken;
        }        
    }

    public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand>
    {
        private readonly ILogger<AcceptInvitationCommandHandler> logger;

        private readonly IInvitationRepository invitationRepository;
        private readonly IUserRepository userRepository;
        private readonly INotificationService notificationService;
        private readonly IActivityRepository activityRepository;
        private readonly ITranslationService translationService;
        private readonly UserStatisticUpdateService rankingService;

        public AcceptInvitationCommandHandler(
            IInvitationRepository invitationRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IActivityRepository activityRepository,
            ITranslationService translationService,
            UserStatisticUpdateService rankingService,
            ILogger<AcceptInvitationCommandHandler> logger)
        {
            this.invitationRepository = invitationRepository;
            this.userRepository = userRepository;
            this.notificationService = notificationService;
            this.activityRepository = activityRepository;
            this.translationService = translationService;
            this.rankingService = rankingService;

            this.logger = logger;
        }

        public async Task<Unit> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
        {
            var invitation = await this.invitationRepository.GetAsync(request.InvitationToken);
            if (request.AcceptingUserId != invitation.InvitingUserId)
            {
                var invitingUser = await this.userRepository.GetUserAsync(invitation.InvitingUserId);
                var acceptingUser = await this.userRepository.GetUserAsync(request.AcceptingUserId);

                if (invitingUser != null && acceptingUser != null)
                {
                    invitingUser.AcceptInvitation(request.InvitationToken, acceptingUser.ToUserInfo());
                    acceptingUser.AddFriend(invitingUser.ToUserInfo());

                    await userRepository.UpdateUserAsync(invitingUser.ToEntity());
                    await userRepository.UpdateUserAsync(acceptingUser.ToEntity());
                }

                try
                {
                    await this.rankingService.IncreaseScoreAsync(invitingUser.Id, Scores.FriendInvitation);

                    var message = await translationService.GetTranslationAsync(invitingUser.Language, "RecruitmentActivityMessage", acceptingUser.Name, Constants.Scores.FriendInvitation);

                    var notificationActivity = Activity.Domain.Activity.CreateNotificationActivity(invitingUser.Id, invitingUser.Name, message);

                    await this.activityRepository.AddActivityAsync(notificationActivity.ToEntity());
                }
                catch (Exception ex)
                {
                    // log error
                    logger.LogError($"Error increasing score for user {invitingUser}: {ex}");
                }

                if (invitingUser != null && invitingUser.PushInfo != null)
                {
                    string userName = acceptingUser?.Name ?? await translationService.GetTranslationAsync(invitingUser.Language, "Somebody");
                    string messageContent = await translationService.GetTranslationAsync(invitingUser.Language, "AcceptedInvitation", userName);

                    var message = new WebPushNotificationMessage(Urls.ApplicationIconUrl, Urls.ApplicationIconUrl, Urls.ApplicationUrl,
                        "Binge Buddy", messageContent);
                    this.notificationService.SendWebPushMessage(new[] { invitingUser.PushInfo }, message);
                }
            }

            return Unit.Value;
        }
    }
}
