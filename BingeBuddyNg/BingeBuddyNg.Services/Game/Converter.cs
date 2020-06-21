using System.Collections.Generic;
using System.Linq;
using BingeBuddyNg.Core.Game.DTO;
using BingeBuddyNg.Core.User;
using BingeBuddyNg.Core.User.DTO;
using BingeBuddyNg.Core.Game.Persistence;

namespace BingeBuddyNg.Core.Game
{
    public static class Converter
    {
        public static GameDTO ToDto(this Domain.Game game, IEnumerable<UserInfoDTO> players)
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
                    }).ToList(),
                WinnerUserId = game.Winner?.UserId
            };
        }

        public static GameDTO ToDto(this GameEntity game)
        {
            return new GameDTO()
            {
                Id = game.Id,
                Title = game.Title,
                UserScores = game.UserScores.Select(gameUserScore => new UserScoreInfoDTO()
                {
                    User = new UserInfoDTO(userId: gameUserScore.User.UserId, userName: gameUserScore.User.UserName),
                    Score = gameUserScore.Score
                }).ToList(),
                Status = game.Status,
                WinnerUserId = game.WinnerUserId
            };
        }

        public static GameEntity ToEntity(this Domain.Game game, IEnumerable<UserInfo> playerUserInfo)
        {
            return new GameEntity()
            {
                Id = game.Id,
                Title = game.Title,
                Status = game.Status,
                WinnerUserId = game.Winner.UserId,
                UserScores = game.Scores.Select(s=>new UserScoreInfo() {  User = playerUserInfo.FirstOrDefault(p=>p.UserId == s.Key ), Score = s.Value}).ToArray()
            };
        }
    }
}
