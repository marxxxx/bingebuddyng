using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace BingeBuddyNg.Services.Activity.Commands
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

        public DeleteActivityCommandHandler(IActivityRepository activityRepository)
        {
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        public async Task<Unit> Handle(DeleteActivityCommand request, CancellationToken cancellationToken)
        {
            await activityRepository.DeleteActivityAsync(request.UserId, request.ActivityId);
            return Unit.Value;
        }
    }
}
