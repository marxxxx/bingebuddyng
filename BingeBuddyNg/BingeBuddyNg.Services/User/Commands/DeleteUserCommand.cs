using BingeBuddyNg.Core.Infrastructure;
using BingeBuddyNg.Core.User.Messages;
using BingeBuddyNg.Shared;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Core.User.Commands
{
    public class DeleteUserCommand : IRequest
    {
        public DeleteUserCommand(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        public string UserId { get; }

        public override string ToString()
        {
            return $"{{{nameof(UserId)}={UserId}}}";
        }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IStorageAccessService storageAccessService;

        public DeleteUserCommandHandler(IStorageAccessService storageAccessService)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
        }

        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            await this.storageAccessService.AddQueueMessage(Constants.QueueNames.DeleteUser, new DeleteUserMessage() { UserId = request.UserId });
            return Unit.Value;
        }
    }
}
