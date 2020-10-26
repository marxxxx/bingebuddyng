using System;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.User.DTO;
using MediatR;

namespace BingeBuddyNg.Core.User.Queries
{
    public class GetUserQuery : IRequest<UserDTO>
    {
        public GetUserQuery(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        public string UserId { get; }
    }

    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDTO>
    {
        private readonly IUserRepository userRepository;

        public GetUserQueryHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<UserDTO> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserAsync(request.UserId);
            return user.ToDto();
        }
    }
}