using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.User.DTO;
using MediatR;

namespace BingeBuddyNg.Core.User.Queries
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
            var result = await userRepository.SearchUsersAsync(filterText: request.FilterText);

            return result.Select(r => r.ToUserInfoDTO()).ToList();
        }
    }
}