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
        public UserQueryHandler(IUserRepository userRepository)
        {
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public IUserRepository UserRepository { get; }

        public async Task<List<UserInfoDTO>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await this.UserRepository.GetUsersAsync();

            var userInfo = users.Select(u => new UserInfoDTO() { UserId = u.Id, UserName = u.Name }).ToList();

            // TODO: Should soon be improved!
            if (!string.IsNullOrEmpty(request.FilterText))
            {
                userInfo = userInfo.Where(u => u.UserName.Contains(request.FilterText)).ToList();
            }

            return userInfo;
        }

        public Task<UserDTO> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
