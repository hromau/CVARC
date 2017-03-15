using System.Linq;
using CvarcWeb.Data;
using CvarcWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace CvarcWeb.Controllers
{
    public class LogsController : Controller
    {
        private readonly CvarcDbContext context;

        public LogsController(CvarcDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetLog(int gameId)
        {
            return File(context.Games.First(g => g.GameId == gameId).PathToLog, "application/octet-stream");
        }
    }
}