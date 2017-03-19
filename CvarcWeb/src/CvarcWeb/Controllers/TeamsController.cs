using System;
using System.Linq;
using CvarcWeb.Data;
using CvarcWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CvarcWeb.Controllers
{
    public class TeamsController : Controller
    {
        private readonly UserDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public TeamsController(UserDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(string name)
        {
            var user = userManager.GetUserAsync(User).Result;
            if (context.Teams.Any(t => t.Name == name))
                return new ContentResult { Content = "NE OK" };
            var created= context.Teams.Add(new Team
            {
                CvarcTag = Guid.NewGuid(),
                OwnerId = user.Id,
                Name = name
            });
            user.Team = created.Entity;
            context.TeamLogs.Add(new TeamLog
            {
                Action = 0,
                Team = created.Entity,
                User = user,
                Time = DateTime.Now
            });

            context.SaveChanges();

            return new ContentResult {Content = "OK"};
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateRequest(string name)
        {
            var user = userManager.GetUserAsync(User).Result;
            var team = context.Teams.First(t => t.Name == name);
            if (team == null || 
                context.TeamRequests.Any(tr => tr.Team.TeamId == team.TeamId && tr.User.Id == user.Id) ||
                team.OwnerId == user.Id)
                return new ContentResult { Content = "NE OK" };
            context.TeamRequests.Add(new TeamRequest { Team = team, User = user});
            context.SaveChanges();
            return new ContentResult {Content = "OK"};
        }

        [Authorize]
        [HttpPost]
        public ActionResult AcceptRequest(string userId)
        {
            var user = userManager.GetUserAsync(User).Result;
            var team = context.Teams.First(t => t.OwnerId == user.Id);
            var request = context.TeamRequests.First(tr => tr.Team.TeamId == team.TeamId && tr.User.Id == userId);
            if (request == null)
                return null;
            context.TeamRequests.Remove(request);
            var joinedUser = context.Users.First(u => u.Id == userId);
            joinedUser.Team = team;

            context.TeamLogs.Add(new TeamLog
            {
                Action = 1,
                Team = team,
                User = joinedUser,
                Time = DateTime.Now
            });

            context.SaveChanges();
            return Content("OK");
        }

        [Authorize]
        [HttpPost]
        public ActionResult LeaveTeam()
        {
            var userId = userManager.GetUserAsync(User).Result.Id;
            var userWithTeam = context.Users.Include(u => u.Team).First(u => u.Id == userId);
            var team = userWithTeam.Team;
            userWithTeam.Team = null;

            context.TeamLogs.Add(new TeamLog
            {
                Action = 2,
                Team = team,
                User = userWithTeam,
                Time = DateTime.Now
            });

            context.SaveChanges();
            return Content("OK");
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