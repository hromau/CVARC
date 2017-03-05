using System;
using System.Collections.Generic;
using System.Linq;
using CvarcWeb.Data;
using CvarcWeb.Models;
using Newtonsoft.Json;
using NuGet.Protocol.Core.v3;

namespace CvarcWeb.Services
{
    public class TournamentGenerator
    {
        private readonly Random random;
        private readonly CvarcDbContext context;
        public TournamentGenerator(CvarcDbContext context, Random random)
        {
            this.context = context;
            this.random = random;
        }

        public int GenerateOlympic(string name, int teamsCount)
        {
            var teamNames = Enumerable.Range(1, teamsCount).Select(i => $"team №{i}").ToArray();
            var stages = new List<List<Game>> { new List<Game>() };
            var stageIndex = PlayMatches(teamsCount, teamNames, stages);
            var tournament = BuildTournament(stages, stageIndex);
            var tournamentJson = tournament.ToJson();
            var generatedTournament = context.Tournaments.Add(new Tournament {Name = name, TournamentTree = tournamentJson, Type = TournamentType.Olympic}).Entity;
            context.SaveChanges();
            return generatedTournament.TournamentId;
        }

        public int GenerateGroup(string name, int teamsCount)
        {
            var teamNames = Enumerable.Range(1, teamsCount).Select(i => $"team №{i}").ToArray();
            var group = new GroupTournament
                        {
                            GameIds = Enumerable.Range(0, teamsCount).Select(_ => new int[teamsCount]).ToArray()
                        };
            for (var i = 0; i < teamsCount; i++)
                for (var j = 0; j < teamsCount; j++)
                    if (i != j)
                        if (i < j)
                            group.GameIds[i][j] = GenerateMatchOfTeams(teamNames[i], teamNames[j]).GameId;
                        else
                            group.GameIds[i][j] = group.GameIds[j][i];
            var generatedTournament =
                context.Tournaments.Add(new Tournament
                {
                    Name = name,
                    TournamentTree = group.ToJson(),
                    Type = TournamentType.Group
                }).Entity;
            context.SaveChanges();
            return generatedTournament.TournamentId;
        }

        private OlympicTournament BuildTournament(List<List<Game>> stages, int stageIndex)
        {
            var tournament = new OlympicTournament
            {
                FinalMatch =
                    new OlympicTournamentMatch(
                        GenerateMatchOfTeams(
                            GetWinner(stages.Last()[0]),
                            GetWinner(stages.Last()[1])).GameId),
                ThirdPlaceMatch =
                    new OlympicTournamentMatch(
                        GenerateMatchOfTeams(
                            GetLooser(stages.Last()[0]),
                            GetLooser(stages.Last()[1])).GameId)
            };
            tournament.FinalMatch.FirstPreviousStageMatch = new OlympicTournamentMatch(stages.Last()[0].GameId);
            tournament.FinalMatch.SecondPreviousStageMatch = new OlympicTournamentMatch(stages.Last()[1].GameId);
            var queue = new Queue<Tuple<OlympicTournamentMatch, string, string, int>>();
            queue.Enqueue(Tuple.Create(tournament.FinalMatch.FirstPreviousStageMatch,
                stages.Last()[0].TeamGameResults.ToArray()[0].Team.Name,
                stages.Last()[0].TeamGameResults.ToArray()[1].Team.Name,
                stageIndex));
            queue.Enqueue(Tuple.Create(tournament.FinalMatch.SecondPreviousStageMatch,
                stages.Last()[1].TeamGameResults.ToArray()[0].Team.Name,
                stages.Last()[1].TeamGameResults.ToArray()[1].Team.Name,
                stageIndex));
            while (queue.Any())
            {
                var currentMatch = queue.Dequeue();
                if (currentMatch.Item4 == 0)
                    continue;
                var firstPrevGame = stages[currentMatch.Item4 - 1].First(
                    g => g.TeamGameResults.Any(tgr => tgr.Team.Name == currentMatch.Item2));
                var secondPrevGame = stages[currentMatch.Item4 - 1].First(
                    g => g.TeamGameResults.Any(tgr => tgr.Team.Name == currentMatch.Item3));
                currentMatch.Item1.FirstPreviousStageMatch =
                    new OlympicTournamentMatch(firstPrevGame.GameId);
                currentMatch.Item1.SecondPreviousStageMatch =
                    new OlympicTournamentMatch(secondPrevGame.GameId);
                queue.Enqueue(Tuple.Create(currentMatch.Item1.FirstPreviousStageMatch,
                              firstPrevGame.TeamGameResults.ToArray()[0].Team.Name,
                              firstPrevGame.TeamGameResults.ToArray()[1].Team.Name, currentMatch.Item4 - 1));
                queue.Enqueue(Tuple.Create(currentMatch.Item1.SecondPreviousStageMatch,
                              secondPrevGame.TeamGameResults.ToArray()[0].Team.Name,
                              secondPrevGame.TeamGameResults.ToArray()[1].Team.Name, currentMatch.Item4 - 1));

            }

            return tournament;
        }

        private int PlayMatches(int teamsCount, string[] teamNames, List<List<Game>> stages)
        {
            for (var i = 0; i < teamsCount; i += 2)
            {
                stages[0].Add(GenerateMatchOfTeams(teamNames[i], teamNames[i + 1]));
            }
            var stageSize = teamsCount / 2;
            var stageIndex = 0;
            while (stageSize != 2)
            {
                stages.Add(new List<Game>());
                for (var j = 0; j < stageSize; j += 2)
                {
                    stages[stageIndex + 1].Add(GenerateMatchOfTeams(GetWinner(stages[stageIndex][j]), GetWinner(stages[stageIndex][j + 1])));
                }
                stageSize /= 2;
                stageIndex++;
            }

            return stageIndex;
        }

        public static string GetWinner(Game game) =>
            game.TeamGameResults.SelectMany(gr => gr.Results)
                                .Where(r => r.ScoresType == "MainScores")
                                .OrderByDescending(r => r.Scores)
                                .First()
                                .TeamGameResult.Team.Name;

        public static string GetLooser(Game game) =>
            game.TeamGameResults.SelectMany(gr => gr.Results)
                                .Where(r => r.ScoresType == "MainScores")
                                .OrderBy(r => r.Scores)
                                .First()
                                .TeamGameResult.Team.Name;

        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private Game GenerateMatchOfTeams(string team1, string team2)
        {
            var gameResult = new Game { GameName = RandomString(random.Next(8, 20)), PathToLog = "http://kek.com" };
            var firstTeam = context.Teams.FirstOrDefault(t => t.Name == team1) ?? new Team { CvarcTag = team1, Name = team1 };
            var secondTeam = context.Teams.FirstOrDefault(t => t.Name == team2) ?? new Team { CvarcTag = team2, Name = team2 };
            var firstTeamGameResult = new TeamGameResult { Team = firstTeam, Game = gameResult };
            var secondTeamGameResult = new TeamGameResult { Team = secondTeam, Game = gameResult };
            var result1 = new Result { TeamGameResult = firstTeamGameResult, Scores = random.Next(100), ScoresType = "MainScores" };
            var result2 = new Result { TeamGameResult = firstTeamGameResult, Scores = random.Next(100), ScoresType = "OtherScores" };
            var result3 = new Result { TeamGameResult = secondTeamGameResult, Scores = random.Next(100), ScoresType = "MainScores" };
            var result4 = new Result { TeamGameResult = secondTeamGameResult, Scores = random.Next(100), ScoresType = "OtherScores" };
            var result = context.Games.Add(gameResult);
            if (!context.Teams.Any(t => t.TeamId == firstTeam.TeamId))
            {
                context.Teams.Add(firstTeam);
            }
            if (!context.Teams.Any(t => t.TeamId == secondTeam.TeamId))
            {
                context.Teams.Add(secondTeam);
            }
            context.TeamGameResults.Add(firstTeamGameResult);
            context.TeamGameResults.Add(secondTeamGameResult);
            context.Results.Add(result1);
            context.Results.Add(result2);
            context.Results.Add(result3);
            context.Results.Add(result4);
            context.SaveChanges();
            return result.Entity;
        }
    }
}
