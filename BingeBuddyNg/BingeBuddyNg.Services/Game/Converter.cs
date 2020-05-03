using BingeBuddyNg.Services.Game.Queries;
using BingeBuddyNg.Services.User;
using System.Collections.Generic;
using System.Linq;

namespace BingeBuddyNg.Services.Game
{
    public static class Converter
    {
        public static GameDTO ToDto(this Game game, IEnumerable<UserInfoDTO> players)
        {
            return new GameDTO()
            {
                Id = game.Id,
                Title = game.Title,
                Status = game.Status,
                UserScores = players.Select(p =>
                    new UserScoreInfoDTO()
                    {
                        User = p,
                        Score = game.Scores.ContainsKey(p.UserId) ? game.Scores[p.UserId] : 0
                    }).ToList()
            };
        }
    }
}
