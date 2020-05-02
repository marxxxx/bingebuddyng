using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.User.Querys
{
    public class GetAllUsersQuery : IRequest<List<UserInfoDTO>>
    {
        public GetAllUsersQuery(string filterText)
        {
            FilterText = filterText;
        }

        public string FilterText { get; }
    }

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserInfoDTO>>
    {
        private readonly IUserRepository userRepository;

        public GetAllUsersQueryHandler(IUserRepository userRepository)
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
    }
}
