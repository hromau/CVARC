using System;
using System.Linq;
using CvarcWeb.Data;
using CvarcWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace CvarcWeb.Controllers
{
    public class TeamsController : Controller
    {
        private readonly CvarcDbContext context;

        public TeamsController(CvarcDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public JsonResult Index(string teamNamePrefix)
        {
            if (string.IsNullOrEmpty(teamNamePrefix))
                return new JsonResult(new {teams = new string[0]});
            var teams = context
                            .Teams
                            .Where(t => t.Name.StartsWith(teamNamePrefix, StringComparison.CurrentCultureIgnoreCase))
                            .Select(t => t.Name)
                            .Take(5)
                            .ToArray();
            return new JsonResult(new { teams });
        }

        [HttpGet]
        public JsonResult GetAllCvarcTags(string apiKey)
        {
            return new JsonResult(apiKey == "huj" ? context.Teams.Select(t => t.CvarcTag).ToArray() : new Guid[0]);
        }
    }
}