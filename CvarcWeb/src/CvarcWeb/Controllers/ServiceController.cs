using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CvarcWeb.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private const string pathToUpdateDir = "Update\\";

        public ServiceController(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult GetVersion()
        {
            return File(pathToUpdateDir + "version", "text/plain");
        }

        [HttpGet]
        public IActionResult GetUpdate()
        {
            return File(pathToUpdateDir + "update.zip", "application/x-zip-compressed");
        }
    }
}