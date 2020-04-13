using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.User.Querys
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
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<UserDTO> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await this.userRepository.FindUserAsync(request.UserId);
            if (user == null)
            {
                return null;
            }

            return new UserDTO()
            {
                Id = user.Id,
                Name = user.Name,
                PushInfo = user.PushInfo,
                ProfileImageUrl = user.ProfileImageUrl,
                Gender = user.Gender,
                Language = user.Language,
                LastOnline = user.LastOnline,
                Weight = user.Weight,
                CurrentVenue = user.CurrentVenue,
                Friends = user.Friends,
                MutedFriendUserIds = user.MutedFriendUserIds
            };
        }
    }
}
