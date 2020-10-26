using System;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity.Messages;
using BingeBuddyNg.Core.Infrastructure;
using MediatR;
using static BingeBuddyNg.Shared.Constants;

namespace BingeBuddyNg.Core.Activity.Commands
{
    public class DeleteActivityCommand : IRequest
    {
        public string UserId { get; }
        public string ActivityId { get; }

        public DeleteActivityCommand(string userId, string activityId)
        {
            this.UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            this.ActivityId = activityId ?? throw new ArgumentNullException(nameof(activityId));
        }
    }

    public class DeleteActivityCommandHandler : IRequestHandler<DeleteActivityCommand>
    {
        private readonly IActivityRepository activityRepository;
        private readonly IStorageAccessService storageAccessService;

        public DeleteActivityCommandHandler(IActivityRepository activityRepository, IStorageAccessService storageAccessService)
        {
            this.activityRepository = activityRepository;
            this.storageAccessService = storageAccessService;
        }

        public async Task<Unit> Handle(DeleteActivityCommand request, CancellationToken cancellationToken)
        {
            await this.activityRepository.DeleteActivityAsync(request.UserId, request.ActivityId);

            // Delete activity in personalized feeds
            await storageAccessService.AddQueueMessage(QueueNames.DeleteActivity, new DeleteActivityMessage(request.ActivityId));

            return Unit.Value;
        }
    }
}