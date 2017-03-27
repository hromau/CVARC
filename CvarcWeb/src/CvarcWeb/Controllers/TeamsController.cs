using System;
using System.Linq;
using CvarcWeb.Data;
using CvarcWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CvarcWeb.Controllers
{
    [Authorize]
    public class TeamsController : Controller
    {
        private readonly UserDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public TeamsController(UserDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [HttpPost]
        public ActionResult Create(string name)
        {
            var user = userManager.GetUserAsync(User).Result;
            if (context.Teams.Any(t => t.Name == name))
                return RedirectToAction(nameof(ManageController.Index), "Manage", new { Message = ManageController.ManageMessageId.TeamAlreadyExistsError });
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
            return RedirectToAction(nameof(ManageController.Index), "Manage", new {Message = ManageController.ManageMessageId.CreateTeamSuccess});
        }

        [HttpPost]
        public ActionResult CreateRequest(string name)
        {
            var user = userManager.GetUserAsync(User).Result;
            var team = context.Teams.FirstOrDefault(t => t.Name == name);
            if (team == null || 
                context.TeamRequests.Any(tr => tr.Team.TeamId == team.TeamId && tr.User.Id == user.Id) ||
                team.OwnerId == user.Id)
                return RedirectToAction(nameof(ManageController.Index), "Manage", new { Message = ManageController.ManageMessageId.CreateRequestError });
            context.TeamRequests.Add(new TeamRequest { Team = team, User = user});
            context.SaveChanges();
            return RedirectToAction(nameof(ManageController.Index), "Manage", new { Message = ManageController.ManageMessageId.CreateRequestSuccess });
        }

        [HttpPost]
        public ActionResult AcceptRequest(string userId)
        {
            var user = userManager.GetUserAsync(User).Result;
            var team = context.Teams.First(t => t.OwnerId == user.Id);
            var request = context.TeamRequests.FirstOrDefault(tr => tr.Team.TeamId == team.TeamId && tr.User.Id == userId);
            if (request == null)
                return RedirectToAction(nameof(ManageController.Index), "Manage");
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
            return RedirectToAction(nameof(ManageController.Index));
        }

        [HttpGet]
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
            return RedirectToAction(nameof(ManageController.Index));
        }

        [HttpGet]
        [Route("Teams/{teamNamePrefix}")]
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