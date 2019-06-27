using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Activity.Querys
{
    public class GetDrinkActivityAggregationQuery : IRequest<List<ActivityAggregationDTO>>
    {

        public GetDrinkActivityAggregationQuery(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        public string UserId { get; }

        public override string ToString()
        {
            return $"{{{nameof(UserId)}={UserId}}}";
        }
    }
}
