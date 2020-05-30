using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Activity.Querys
{
    public class GetActivitysForMapQuery : IRequest<List<ActivityDTO>>
    {
        public string UserId { get; }

        public GetActivitysForMapQuery(string userId)
        {
            this.UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }
    }

    public class GetActivitysForMapQueryHandler : IRequestHandler<GetActivitysForMapQuery, List<ActivityDTO>>
    {
        private readonly IActivityRepository activityRepository;

        public GetActivitysForMapQueryHandler(IActivityRepository activityRepository)
        {
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        public async Task<List<ActivityDTO>> Handle(GetActivitysForMapQuery request, CancellationToken cancellationToken)
        {
            var args = new GetActivityFilterArgs() { UserId = request.UserId, FilterOptions = ActivityFilterOptions.WithLocation, PageSize = 50 };
            var result = await this.activityRepository.GetActivityFeedAsync(args);
            return result.ResultPage?.Select(a => a.ToDto()).ToList();
        }
    }
}
