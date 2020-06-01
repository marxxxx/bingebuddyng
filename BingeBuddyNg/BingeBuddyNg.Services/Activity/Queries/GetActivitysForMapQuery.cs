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
        public GetActivitysForMapQuery()
        {
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
            var args = new GetActivityFilterArgs() { FilterOptions = ActivityFilterOptions.WithLocation, PageSize = 50 };
            var result = await this.activityRepository.GetMasterActivitiesAsync(args);
            return result.ToList();
        }
    }
}
