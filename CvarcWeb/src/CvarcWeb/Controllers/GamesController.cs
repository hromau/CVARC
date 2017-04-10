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
                            .Where(g => string.IsNullOrEmpty(filters.TeamName) || g.TeamGameResults.Any(gr => gr.Team.Name != null && gr.Team.Name.StartsWith(filters.TeamName, StringComparison.CurrentCultureIgnoreCase)))
                            .OrderByDescending(g => g.GameId)
                            .AsQueryable();
            if (!filters.GameId.HasValue)
                return foundGames;
            return foundGames.Where(g => g.GameId == filters.GameId.Value);
        }

        private static Game[] GetPage(GameFilterModel model, IQueryable<Game> filteredGames) => 
            filteredGames.Skip(model.Page * GamesPerPage)
                         .Take(GamesPerPage)
                         .ToArray()
                         .Select(g =>
                         {
                             g.PathToLog = null;
                             g.TeamGameResults.ToList().ForEach(r =>
                             {
                                 r.Team.CvarcTag = Guid.Empty;
                             });
                             return g;
                         }).ToArray();
    }
}
