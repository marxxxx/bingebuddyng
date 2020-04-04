using BingeBuddyNg.Services.Drink;
using BingeBuddyNg.Services.Infrastructure;
using BingeBuddyNg.Services.Statistics;
using BingeBuddyNg.Services.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services.Activity.Querys
{
    public static class GetDrinkActivityAggregation
    {
        public class Query : IRequest<List<ActivityAggregationDTO>>
        {
            public Query(string userId)
            {
                UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            }

            public string UserId { get; }

            public override string ToString()
            {
                return $"{{{nameof(UserId)}={UserId}}}";
            }
        }

        public class Handler : IRequestHandler<Query, List<ActivityAggregationDTO>>
        {
            private readonly IActivityRepository activityRepository;

            public Handler(IActivityRepository activityRepository)
            {
                this.activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            }

            public async Task<List<ActivityAggregationDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                var startTime = DateTime.UtcNow.AddDays(-30).Date;

                var result = await this.activityRepository.GetActivitysForUserAsync(request.UserId, startTime, ActivityType.Drink);

                var groupedByDay = result.GroupBy(t => t.Timestamp.Date)
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
}
