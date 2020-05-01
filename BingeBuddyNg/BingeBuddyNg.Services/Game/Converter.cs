using BingeBuddyNg.Services.Game.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingeBuddyNg.Services.Game
{
    public static class Converter
    {
        public static GameStatusDTO ToDto(this Game game)
        {
            return new GameStatusDTO()
            {
                Id = game.Id,
                Title = game.Title,
                Scores = game.Scores.Select(gameScore => new UserScore(gameScore.Key, gameScore.Value)).ToList()
            };
        }
    }
}
