using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CvarcWeb.Data;

namespace CvarcWeb.Models
{
    // При изменении этой модели нужно обязательно поменять модель в Infrastruction.
    public class WebCommonResults
    {
        public string GameName { get; set; }
        public string PathToLog { get; set; }
        public Dictionary<string, Guid> RoleToCvarcTag { get; set; }
        public Dictionary<string, int> RoleToTeamId { get; set; }
        public Dictionary<string, Dictionary<string, int>> Scores { get; set; }
    }

    public static class WebCommonResultExtensions
    {
        public static void SaveToDb(this WebCommonResults result, UserDbContext context)
        {
            var game = new Game
            {
                GameName = result.GameName,
                PathToLog = result.PathToLog,
                TeamGameResults = new List<TeamGameResult>()
            };
            foreach (var kvp in result.Scores)
            {
                var teamTag = result.RoleToCvarcTag[kvp.Key];
                var team = context.Teams.Single(t => t.CvarcTag == teamTag);
                var teamResult = new TeamGameResult
                {
                    Role = kvp.Key,
                    Team = team,
                    Game = game,
                    Results = new List<Result>()
                };
                foreach (var score in kvp.Value)
                {
                    var scoreResult = new Result
                    {
                        TeamGameResult = teamResult,
                        ScoresType = score.Key,
                        Scores = score.Value
                    };
                    teamResult.Results.Add(scoreResult);
                    context.Results.Add(scoreResult);
                }
                game.TeamGameResults.Add(teamResult);
                context.TeamGameResults.Add(teamResult);
            }
            context.Games.Add(game);
            context.SaveChanges();
        }

        public static int SaveToDbAndGetGameId(this WebCommonResults result, UserDbContext context)
        {
            var game = new Game
            {
                GameName = result.GameName,
                PathToLog = result.PathToLog,
                TeamGameResults = new List<TeamGameResult>()
            };
            foreach (var kvp in result.Scores)
            {
                var teamId = result.RoleToTeamId[kvp.Key];
                var team = context.Teams.Single(t => t.TeamId == teamId);
                var teamResult = new TeamGameResult
                {
                    Role = kvp.Key,
                    Team = team,
                    Game = game,
                    Results = new List<Result>()
                };
                foreach (var score in kvp.Value)
                {
                    var scoreResult = new Result
                    {
                        TeamGameResult = teamResult,
                        ScoresType = score.Key,
                        Scores = score.Value
                    };
                    teamResult.Results.Add(scoreResult);
                    context.Results.Add(scoreResult);
                }
                game.TeamGameResults.Add(teamResult);
                context.TeamGameResults.Add(teamResult);
            }
            context.Games.Add(game);
            context.SaveChanges();
            return game.GameId;
        }
    }
}
