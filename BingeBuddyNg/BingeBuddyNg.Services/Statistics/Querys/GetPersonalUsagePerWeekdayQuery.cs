using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Statistics.Querys
{
    public class GetPersonalUsagePerWeekdayQuery : IRequest<IEnumerable<PersonalUsagePerWeekdayDTO>>
    {
        public GetPersonalUsagePerWeekdayQuery(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        public string UserId { get; }
    }
}
