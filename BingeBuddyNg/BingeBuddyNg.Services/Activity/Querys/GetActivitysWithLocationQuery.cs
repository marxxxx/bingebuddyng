using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Activity.Querys
{
    public class GetActivitysWithLocationQuery : IRequest<List<ActivityDTO>>
    {
        public string UserId { get; }

        public GetActivitysWithLocationQuery(string userId)
        {
            this.UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }
    }
}
