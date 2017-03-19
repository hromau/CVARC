using System;
using System.Linq;
using CvarcWeb.Data;
using CvarcWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CvarcWeb.Controllers
{
    public class TeamsController : Controller
    {
        private readonly CvarcDbContext context;
        private readonly UserDbContext userContext;
        private readonly UserManager<ApplicationUser> userManager;

        public TeamsController(CvarcDbContext context, UserManager<ApplicationUser> userManager, UserDbContext userContext)
        {
            this.context = context;
            this.userManager = userManager;
            this.userContext = userContext;
        }

        [Authorize]
        [HttpGet]
        public ActionResult Create(string name)
        {
            var user = userManager.GetUserAsync(User).Result;
            if (context.Teams.Any(t => t.Name == name))
                return new ContentResult { Content = "NE OK" };
            var created= context.Teams.Add(new Team
            {
                CvarcTag = Guid.NewGuid(),
                Owner = user,
                Name = name
            });
            context.SaveChanges();
            user.Team = created.Entity;
            userContext.SaveChanges();

            return new ContentResult {Content = "OK"};
        }

        public ActionResult TryJoin(string name)
        {
            var user = userManager.GetUserAsync(User).Result;
            //var team = context.Teams.First(t => t.Name == name);
            //if (team == null || context.TeamRequests.Any(tr => tr.Team.TeamId == team.TeamId && tr.UserId.ToString() == user.Id))
            //    return new ContentResult { Content = "NE OK" };
            //context.TeamRequests.Add(new TeamRequest {Team = team, });
            return new ContentResult {Content = user.Team.ToString()};
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