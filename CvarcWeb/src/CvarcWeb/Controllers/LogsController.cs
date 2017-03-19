using System.IO;
using System.Linq;
using CvarcWeb.Data;
using CvarcWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace CvarcWeb.Controllers
{
    public class LogsController : Controller
    {
        private readonly UserDbContext context;

        public LogsController(UserDbContext context)
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
            var pathToLog = context.Games.First(g => g.GameId == gameId).PathToLog;
            var content = System.IO.File.ReadAllBytes(pathToLog);
            return File(content, "application/octet-stream", Path.GetFileName(pathToLog));
        }
    }
}