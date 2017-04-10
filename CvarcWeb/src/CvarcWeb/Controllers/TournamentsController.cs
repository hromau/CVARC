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
        private readonly UserDbContext context;
        private readonly TournamentGenerator tournamentGenerator;
        private readonly GamesRepository gamesRepository;
        public TournamentsController(UserDbContext context,
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
            if (string.IsNullOrEmpty(tournamentName))
            {
                return View();
            }
            
            if (tournamentName.Equals("all", StringComparison.CurrentCultureIgnoreCase))
            {
                if (!User.IsInRole("admin"))
                    return new JsonResult(new string[0]);
                return new JsonResult(context.Tournaments.Select(t => t.Name).ToList());
            }
            var tournament =
                context.Tournaments.FirstOrDefault(
                    t => t.Name.Equals(tournamentName, StringComparison.CurrentCultureIgnoreCase));
            if (tournament == null)
                return View();

            return
                new JsonResult(MapTournamentToViewModel(tournament), new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
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
                    tournamentViewModel = MapGroupTournament(JsonConvert.DeserializeObject<GroupTournament>(t.TournamentTree));
                    break;
                default:
                    return null;
            }
            tournamentViewModel.Type = t.Type;
            return tournamentViewModel;
        }

        private GroupTournamentViewModel MapGroupTournament(GroupTournament groupTournament)
        {
            return new GroupTournamentViewModel
            {
                Type = TournamentType.Group,
                Groups = groupTournament.GameIds.Select(MapGroup).ToArray()
            };
        }

        private GroupViewModel MapGroup(int[][] group, int groupNumber)
        {
            var gameIds = group.SelectMany(row => row).ToArray();
            var idToGameMap = gamesRepository.GetByIds(gameIds).ToDictionary(g => g.GameId, g => g);
            var groupSize = group.Length;
            var groupViewModel = new GroupViewModel
            {
                Games = Enumerable.Range(0, groupSize)
                                  .Select(_ => new Game[groupSize])
                                  .ToArray()
            };
            for (var i = 0; i < groupSize; i++)
                for (var j = 0; j < groupSize; j++)
                    if (i != j)
                        groupViewModel.Games[i][j] = idToGameMap[group[i][j]];
            groupViewModel.GroupName = $"Group №{groupNumber + 1}";
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
            var result = new Dictionary<string, int>();
            for (var i = 0; i < olympicTournamentsCount; i++)
                result[$"Test olympic tournament №{i}"] = tournamentGenerator.GenerateOlympic($"Test olympic tournament №{i}", 8);
            for (var i = 0; i < groupTournamentsCount; i++)
                result[$"Test group tournament №{i + groupTournamentsCount}"] = tournamentGenerator.GenerateGroup($"Test group tournament №{i+ groupTournamentsCount}", 4);
        }
    }
}