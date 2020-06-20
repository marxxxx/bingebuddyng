using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BingeBuddyNg.Services.Drink;
using MediatR;

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

    public class GetDrinkActivityAggregationQueryHandler : IRequestHandler<GetDrinkActivityAggregationQuery, List<ActivityAggregationDTO>>
    {
        private readonly IActivityRepository activityRepository;

        public GetDrinkActivityAggregationQueryHandler(IActivityRepository activityRepository)
        {
            this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        }

        public async Task<List<ActivityAggregationDTO>> Handle(GetDrinkActivityAggregationQuery request, CancellationToken cancellationToken)
        {
            var startTime = DateTime.UtcNow.AddDays(-30).Date;

            var result = await this.activityRepository.GetUserActivitiesAsync(request.UserId, startTime, ActivityType.Drink);

            var groupedByDay = result.Where(r=>r.ActivityType == ActivityType.Drink).GroupBy(t => t.Timestamp.Date)
                .OrderBy(t => t.Key)
                .Select(t => new ActivityAggregationDTO()
                {
                    Count = t.Count(),
                    CountBeer = t.Count(d => d.DrinkType == DrinkType.Beer),
                    CountWine = t.Count(d => d.DrinkType == DrinkType.Wine),
                    CountShots = t.Count(d => d.DrinkType == DrinkType.Shot),
                    CountAnti = t.Count(d => d.DrinkType == DrinkType.Anti),
                    CountAlc = t.Count(d => d.DrinkType != DrinkType.Anti),
                    Day = t.Key
                })
                .ToList();

            // now fill holes of last 30 days
            for (int i = -30; i < 0; i++)
            {
                var date = DateTime.UtcNow.AddDays(i).Date;
                var hasData = groupedByDay.Any(d => d.Day == date);
                if (hasData == false)
                {
                    groupedByDay.Add(new ActivityAggregationDTO(date));
                }
            }

            var sortedResult = groupedByDay.OrderBy(d => d.Day).ToList();

            return sortedResult;
        }
    }
}
