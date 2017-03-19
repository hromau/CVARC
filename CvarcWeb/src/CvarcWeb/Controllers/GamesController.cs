using System;
using System.Linq;
using System.Text;
using CvarcWeb.Data;
using CvarcWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CvarcWeb.Controllers
{
    public class GamesController : Controller
    {
        private static readonly Random random = new Random(0);
        private readonly UserDbContext context;
        private const int GamesPerPage = 30;

        public GamesController(UserDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Find(GameFilterModel filters)
        {
            var games = GetGames(filters);
            var total = games.Count();
            return new JsonResult(new {games = GetPage(filters, games), total},
                new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
        }

        [HttpPut]
        public ActionResult Add(string apiKey)
        {
            if (apiKey != WebConstants.ApiKey)
                return new JsonResult(new { success = false });
            try
            {
                var request = HttpContext.Request;
                var contentLength = (int)request.ContentLength.Value;
                var data = new byte[contentLength];
                request.Body.Read(data, 0, contentLength);
                var json = Encoding.UTF8.GetString(data);
                var result = JsonConvert.DeserializeObject<WebCommonResults>(json);
                result.SaveToDb(context);
                return new JsonResult(new { success = true });
            }
            catch (Exception)
            {
                return new JsonResult(new { success = false });
            }
        }

        private IQueryable<Game> GetGames(GameFilterModel filters)
        {
            var foundGames = context.Games
                            .Include(g => g.TeamGameResults).ThenInclude(cgr => cgr.Team)
                            .Include(g => g.TeamGameResults).ThenInclude(cgr => cgr.Results)
                            .Where(g => string.IsNullOrEmpty(filters.GameName) || g.GameName == filters.GameName)
                            .Where(g => string.IsNullOrEmpty(filters.TeamName) || g.TeamGameResults.Any(gr => gr.Team.Name.StartsWith(filters.TeamName, StringComparison.CurrentCultureIgnoreCase)))
                            .AsQueryable();
            if (!filters.GameId.HasValue)
                return foundGames;
            return foundGames.Where(g => g.GameId == filters.GameId.Value);
        }

        private static Game[] GetPage(GameFilterModel model, IQueryable<Game> filteredGames) => 
            filteredGames.Skip(model.Page * GamesPerPage)
                         .Take(GamesPerPage)
                         .ToArray();

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpGet]
        private IActionResult CreateTestDb()
        {
            for (var i = 0; i < 100; i++)
                AddRandomData();
            return new ContentResult { Content = "yep!" };
        }

        private void AddRandomData()
        {
            var gameResult = new Game { GameName = RandomString(random.Next(8, 20)), PathToLog = "C:/" };
            var firstTeam = new Team { CvarcTag = Guid.NewGuid(), LinkToImage = "qwe", Name = RandomString(random.Next(8, 20)) };
            var secondTeam = new Team { CvarcTag = Guid.NewGuid(), LinkToImage = "qwer", Name = RandomString(random.Next(8, 20)) };
            var firstTeamGameResult = new TeamGameResult { Team = firstTeam, Game = gameResult };
            var secondTeamGameResult = new TeamGameResult { Team = secondTeam, Game = gameResult };
            var result1 = new Result { TeamGameResult = firstTeamGameResult, Scores = random.Next(100), ScoresType = "MainScores" };
            var result2 = new Result { TeamGameResult = firstTeamGameResult, Scores = random.Next(100), ScoresType = "OtherScores" };
            var result3 = new Result { TeamGameResult = secondTeamGameResult, Scores = random.Next(100), ScoresType = "MainScores" };
            var result4 = new Result { TeamGameResult = secondTeamGameResult, Scores = random.Next(100), ScoresType = "OtherScores" };
            context.Games.Add(gameResult);
            context.Teams.Add(firstTeam);
            context.Teams.Add(secondTeam);
            context.TeamGameResults.Add(firstTeamGameResult);
            context.TeamGameResults.Add(secondTeamGameResult);
            context.Results.Add(result1);
            context.Results.Add(result2);
            context.Results.Add(result3);
            context.Results.Add(result4);
            context.SaveChanges();
        }
    }
}
