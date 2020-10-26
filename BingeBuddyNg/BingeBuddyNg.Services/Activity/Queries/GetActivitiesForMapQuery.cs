using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Activity.Domain;
using BingeBuddyNg.Core.Activity.DTO;
using MediatR;

namespace BingeBuddyNg.Core.Activity.Queries
{
    public class GetActivitiesForMapQuery : IRequest<IEnumerable<ActivityDTO>>
    {
    }

    public class GetActivitiesForMapQueryHandler : IRequestHandler<GetActivitiesForMapQuery, IEnumerable<ActivityDTO>>
    {
        private readonly IActivityRepository activityRepository;

        public GetActivitiesForMapQueryHandler(IActivityRepository activityRepository)
        {
            this.activityRepository = activityRepository;
        }

        public async Task<IEnumerable<ActivityDTO>> Handle(GetActivitiesForMapQuery request, CancellationToken cancellationToken)
        {
            var args = new ActivityFilterArgs() { FilterOptions = ActivityFilterOptions.WithLocation, PageSize = 50 };

            var result = await this.activityRepository.GetMasterActivitiesAsync(args);

            return result.Select(r => r.ToDto()).ToList();
        }
    }
}
