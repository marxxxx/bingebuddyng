using BingeBuddyNg.Services.Activity;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;
using BingeBuddyNg.Shared;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Invitation.Commands
{
    public class AcceptInvitationCommand : IRequest
    {
        public AcceptInvitationCommand(string acceptingUserId, string invitationToken)
        {
            AcceptingUserId = acceptingUserId ?? throw new ArgumentNullException(nameof(acceptingUserId));
            InvitationToken = invitationToken ?? throw new ArgumentNullException(nameof(invitationToken));
        }

        public string AcceptingUserId { get; }
        public string InvitationToken { get; }
    }

    public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand>
    {
        private readonly ILogger<AcceptInvitationCommandHandler> logger;

        private readonly IInvitationRepository invitationRepository;
        private readonly IUserRepository userRepository;
        private readonly INotificationService notificationService;
        private readonly IActivityRepository activityRepository;
        private readonly ITranslationService translationService;
        private readonly IUserStatsRepository userStatsRepository;

        public AcceptInvitationCommandHandler(IInvitationRepository invitationRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IActivityRepository activityRepository,
            ITranslationService translationService,
            IUserStatsRepository userStatsRepository,
            ILogger<AcceptInvitationCommandHandler> logger)
        {
            this.invitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.translationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
            this.userStatsRepository = userStatsRepository ?? throw new ArgumentNullException(nameof(userStatsRepository));

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
        {
            var invitation = await this.invitationRepository.AcceptInvitationAsync(request.AcceptingUserId, request.InvitationToken);
            if (request.AcceptingUserId != invitation.InvitingUserId)
            {
                var invitingUser = await this.userRepository.FindUserAsync(invitation.InvitingUserId);
                var acceptingUser = await this.userRepository.FindUserAsync(request.AcceptingUserId);

                if (invitingUser != null && acceptingUser != null)
                {
                    await this.userRepository.AddFriendAsync(invitingUser.Id, acceptingUser.Id);
                }

                try
                {
                    await this.userStatsRepository.IncreaseScoreAsync(invitingUser.Id, Constants.Scores.FriendInvitation);

                    var message = await translationService.GetTranslationAsync(invitingUser.Language, "RecruitmentActivityMessage", acceptingUser.Name, Constants.Scores.FriendInvitation);
                    var notificationActivity = Activity.Activity.CreateNotificationActivity(DateTime.UtcNow, invitingUser.Id, invitingUser.Name, message);

                    await this.activityRepository.AddActivityAsync(notificationActivity);
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

                    var message = new WebPushNotificationMessage(Constants.Urls.ApplicationIconUrl, Constants.Urls.ApplicationIconUrl, Constants.Urls.ApplicationUrl,
                        "Binge Buddy", messageContent);
                    this.notificationService.SendWebPushMessage(new[] { invitingUser.PushInfo }, message);
                }
            }

            return Unit.Value;
        }
    }
}
