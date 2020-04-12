using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.User.Querys
{
    public class UserQueryHandler :
        IRequestHandler<GetAllUsersQuery, List<UserInfoDTO>>,
        IRequestHandler<GetUserQuery, UserDTO>
    {
        private readonly IUserRepository userRepository;

        public UserQueryHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }        

        public async Task<List<UserInfoDTO>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await this.userRepository.GetUsersAsync();

            var userInfo = users.Select(u => new UserInfoDTO() { UserId = u.Id, UserName = u.Name }).ToList();

            // TODO: Should soon be improved!
            if (!string.IsNullOrEmpty(request.FilterText))
            {
                string lowerFilter = request.FilterText.ToLower();
                userInfo = userInfo.Where(u => u.UserName.ToLower().Contains(lowerFilter)).ToList();
            }

            return userInfo;
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
