using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Ranking.Querys
{
    public class GetDrinksRankingQuery : IRequest<List<UserRankingDTO>>
    {
    }
}
