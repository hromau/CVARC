using Microsoft.AspNetCore.Mvc;

namespace CvarcWeb.Controllers
{
    public class LogsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}