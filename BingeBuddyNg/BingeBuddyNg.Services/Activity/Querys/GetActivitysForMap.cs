using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Activity.Querys
{
    public static class GetActivitysForMap
    {
        public class Query : IRequest<List<ActivityDTO>>
        {
            public string UserId { get; }

            public Query(string userId)
            {
                this.UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            }
        }

        public class Handler : IRequestHandler<Query, List<ActivityDTO>>
        {
            private readonly IActivityRepository activityRepository;

            public Handler(IActivityRepository activityRepository)
            {
                this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            }

            public async Task<List<ActivityDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                var result = await this.activityRepository.GetActivityFeedAsync(new GetActivityFilterArgs(request.UserId, ActivityFilterOptions.WithLocation, pageSize: 50));
                return result.ResultPage?.Select(a => a.ToDto()).ToList();
            }
        }
    }
}
