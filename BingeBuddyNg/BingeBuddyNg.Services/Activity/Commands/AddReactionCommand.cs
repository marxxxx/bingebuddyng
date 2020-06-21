using System;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Shared;
using MediatR;
using Newtonsoft.Json;

namespace BingeBuddyNg.Services.Activity.Commands
{
    public class AddReactionCommand : IRequest
    {
        public AddReactionCommand(string userId, ReactionType type, string activityId, string comment)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            ActivityId = activityId ?? throw new ArgumentNullException(nameof(activityId));
            Type = type;
            Comment = comment;
        }

        public string UserId { get; }
        public ReactionType Type { get; }
        public string ActivityId { get; }
        public string Comment { get; }

        public override string ToString()
        {
            return $"{{{nameof(UserId)}={UserId}, {nameof(Type)}={Type}, {nameof(ActivityId)}={ActivityId}, {nameof(Comment)}={Comment}}}";
        }
    }

    public class AddReactionCommandHandler : IRequestHandler<AddReactionCommand>
    {
        private readonly IUserRepository userRepository;
        private readonly IActivityRepository activityRepository;
        private readonly IStorageAccessService storageAccessService;

        public AddReactionCommandHandler(
            IUserRepository userRepository,
            IActivityRepository activityRepository,
            IStorageAccessService storageAccessService)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public async Task<Unit> Handle(AddReactionCommand request, CancellationToken cancellationToken)
        {
            var activity = await this.activityRepository.GetActivityAsync(request.ActivityId);

            var reactingUser = await this.userRepository.GetUserAsync(request.UserId);

            switch (request.Type)
            {

                case ReactionType.Cheers:
                    activity.AddCheers(new Reaction(request.UserId, reactingUser.Name));
                    break;
                case ReactionType.Like:
                    activity.AddLike(new Reaction(request.UserId, reactingUser.Name));
                    break;
                case ReactionType.Comment:
                    activity.AddComment(new CommentReaction(request.UserId, reactingUser.Name, request.Comment));
                    break;
            }

            await this.activityRepository.UpdateActivityAsync(activity.ToEntity());

            // add to queue
            var message = new ReactionAddedMessage(request.ActivityId, request.Type, request.UserId, request.Comment);
            await storageAccessService.AddQueueMessage(Constants.QueueNames.ReactionAdded, message);

            return Unit.Value;
        }
    }
}
