using BingeBuddyNg.Api.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Statistics.Querys
{
    public class GetStatisticHistoryForUserQuery : IRequest<IEnumerable<UserStatisticHistoryDTO>>
    {
        public GetStatisticHistoryForUserQuery(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        public string UserId { get; }
    }
}
