using System;
using System.Collections.Generic;
using System.Linq;
using CvarcWeb.Data;
using CvarcWeb.Data.Repositories;
using CvarcWeb.Models;
using CvarcWeb.Services;
using CvarcWeb.Tournaments.Common;
using CvarcWeb.Tournaments.GroupStage;
using CvarcWeb.Tournaments.Playoff;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CvarcWeb.Controllers
{
    public class TournamentsController : Controller
    {
        private readonly CvarcDbContext context;
        private readonly TournamentGenerator tournamentGenerator;
        private readonly GamesRepository gamesRepository;

        //TODO: Вынести в конфиг маппинг названия турнира в айдишник или похер?
        private static readonly Dictionary<string, int> TournamentMap = new Dictionary<string, int>
        {
            ["TestTournament №0"] = 1093,
            ["TestTournament №1"] = 1094,
            ["TestTournament №2"] = 1095,
            ["TestTournament №3"] = 1096,
            ["TestTournament №4"] = 1097,
            ["TestTournament №5"] = 1098,
            ["TestTournament №6"] = 1099,
            ["TestTournament №7"] = 1100,
            ["TestTournament №8"] = 1101,
            ["TestTournament №9"] = 1102
        }; 

        public TournamentsController(CvarcDbContext context,
                                     TournamentGenerator tournamentGenerator,
                                     GamesRepository gamesRepository)
        {
            this.context = context;
            this.tournamentGenerator = tournamentGenerator;
            this.gamesRepository = gamesRepository;
        }

        [Route(@"Tournaments/{tournamentName}")]
        [Route(@"Tournaments")]
        [HttpGet]
        public IActionResult Index(string tournamentName)
        {
            if (!context.Tournaments.Any())
                ClearAndGenerateTournaments(5, 5);

            if (string.IsNullOrEmpty(tournamentName))
            {
                return View();
            }
            
            if (tournamentName.Equals("all", StringComparison.CurrentCultureIgnoreCase))
            {
                return new JsonResult(TournamentMap.Keys.ToList());
            }

            if (!TournamentMap.ContainsKey(tournamentName))
                return View();

            return
                new JsonResult(
                    context.Tournaments.Where(t => t.TournamentId == TournamentMap[tournamentName])
                                       .Select(MapTournamentToViewModel)
                                       .Single(),
                    new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
        }

        private ITournamentViewModel MapTournamentToViewModel(Tournament t)
        {
            ITournamentViewModel tournamentViewModel;
            switch (t.Type)
            {
                case TournamentType.Olympic:
                    tournamentViewModel = MapOlympic(JsonConvert.DeserializeObject<OlympicTournament>(t.TournamentTree));
                    break;
                case TournamentType.Group:
                    tournamentViewModel = MapGroup(JsonConvert.DeserializeObject<GroupTournament>(t.TournamentTree));
                    break;
                default:
                    return null;
            }
            tournamentViewModel.Name = t.Name;
            tournamentViewModel.Id = t.TournamentId;
            tournamentViewModel.Type = t.Type;
            return tournamentViewModel;
        }

        private GroupTournamentViewModel MapGroup(GroupTournament groupTournament)
        {
            var gameIds = groupTournament.GameIds.SelectMany(row => row).ToArray();
            var idToGameMap = gamesRepository.GetByIds(gameIds).ToDictionary(g => g.GameId, g => g);
            var groupSize = groupTournament.GameIds.Length;
            var groupViewModel = new GroupTournamentViewModel
            {
                Games = Enumerable.Range(0, groupSize)
                                  .Select(_ => new Game[groupSize])
                                  .ToArray()
            };
            for(var i = 0; i < groupSize; i++)
                for (var j = 0; j < groupSize; j++)
                    if (i != j)
                        groupViewModel.Games[i][j] = idToGameMap[groupTournament.GameIds[i][j]];
            return groupViewModel;
        }

        private OlympicTournamentViewModel MapOlympic(OlympicTournament tournament)
        {
            var idToGameMap = GetIdToGameMap(tournament);
            var queue = new Queue<OlympicTournamentMatch>();
            var queueViewModel = new Queue<OlympicTournamentMatchViewModel>();
            var tournamentViewModel = new OlympicTournamentViewModel
            {
                FinalMatch = new OlympicTournamentMatchViewModel(idToGameMap[tournament.FinalMatch.GameId]),
                ThirdPlaceMatch = new OlympicTournamentMatchViewModel(idToGameMap[tournament.ThirdPlaceMatch.GameId])
            };
            queue.Enqueue(tournament.FinalMatch);
            queueViewModel.Enqueue(tournamentViewModel.FinalMatch);
            while (queue.Count != 0)
            {
                var current = queue.Dequeue();
                var currentViewModel = queueViewModel.Dequeue();

                if (current.FirstPreviousStageMatch == null)
                    continue;

                currentViewModel.FirstPreviousStageMatch = new OlympicTournamentMatchViewModel(idToGameMap[current.FirstPreviousStageMatch.GameId]);
                currentViewModel.SecondPreviousStageMatch = new OlympicTournamentMatchViewModel(idToGameMap[current.SecondPreviousStageMatch.GameId]);
                
                queue.Enqueue(current.FirstPreviousStageMatch);
                queue.Enqueue(current.SecondPreviousStageMatch);
                queueViewModel.Enqueue(currentViewModel.FirstPreviousStageMatch);
                queueViewModel.Enqueue(currentViewModel.SecondPreviousStageMatch);
            }
            return tournamentViewModel;
        }

        private Dictionary<int, Game> GetIdToGameMap(OlympicTournament tournament)
        {
            var ids = new List<int> {tournament.ThirdPlaceMatch.GameId};
            var queue = new Queue<OlympicTournamentMatch>();
            queue.Enqueue(tournament.FinalMatch);
            while (queue.Count != 0)
            {
                var current = queue.Dequeue();
                ids.Add(current.GameId);
                if (current.FirstPreviousStageMatch == null)
                    continue;
                queue.Enqueue(current.FirstPreviousStageMatch);
                queue.Enqueue(current.SecondPreviousStageMatch);
            }
            return gamesRepository.GetByIds(ids).ToDictionary(g => g.GameId, g => g);
        }

        private void ClearAndGenerateTournaments(int olympicTournamentsCount, int groupTournamentsCount)
        {
            context.Teams.RemoveRange(context.Teams);
            context.Results.RemoveRange(context.Results);
            context.TeamGameResults.RemoveRange(context.TeamGameResults);
            context.Games.RemoveRange(context.Games);
            context.Tournaments.RemoveRange(context.Tournaments);
            context.SaveChanges();

            var result = new Dictionary<string, int>();
            for (var i = 0; i < olympicTournamentsCount; i++)
                result[$"Test olympic tournament №{i}"] = tournamentGenerator.GenerateOlympic($"Test olympic tournament №{i}", 8);
            for (var i = 0; i < groupTournamentsCount; i++)
                result[$"Test group tournament №{i + groupTournamentsCount}"] = tournamentGenerator.GenerateGroup($"Test group tournament №{i+ groupTournamentsCount}", 4);
        }
    }
}