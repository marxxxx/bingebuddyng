using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

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
}
