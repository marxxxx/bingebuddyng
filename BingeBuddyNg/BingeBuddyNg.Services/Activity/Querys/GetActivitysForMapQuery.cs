using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Activity.Querys
{
    public class GetActivitysForMapQuery : IRequest<List<Activity>>
    {
        public string UserId { get; }

        public GetActivitysForMapQuery(string userId)
        {
            this.UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }
    }
}
