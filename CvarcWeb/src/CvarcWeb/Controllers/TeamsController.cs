using System;
using System.Collections.Generic;
using System.Linq;
using CvarcWeb.Data;
using CvarcWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CvarcWeb.Controllers
{
    [Authorize]
    public class TeamsController : Controller
    {
        private readonly UserDbContext context;

        private readonly Dictionary<string, int> MaxTeamSizes = new Dictionary<string, int>
        {
            ["MathMech"] = 2,
            ["ItPlanet"] = 1
        };

        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public TeamsController(UserDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpPost]
        public ActionResult Create(string name)
        {
            var user = userManager.GetUserAsync(User).Result;
            if (context.Teams.Any(t => t.Name == name))
                return RedirectToAction(nameof(ManageController.Index), "Manage",
                    new {Message = ManageController.ManageMessageId.TeamAlreadyExistsError});
            
            var created = context.Teams.Add(new Team
            {
                CvarcTag = Guid.NewGuid(),
                OwnerId = user.Id,
                Name = name,
                MaxSize =
                    userManager.GetRolesAsync(user).Result.Max(r => MaxTeamSizes.ContainsKey(r) ? MaxTeamSizes[r] : 1),
                CanOwnerLeave = userManager.IsInRoleAsync(user, "MathMech").Result
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
            return RedirectToAction(nameof(ManageController.Index), "Manage",
                new {Message = ManageController.ManageMessageId.CreateTeamSuccess});
        }

        [HttpPost]
        public ActionResult CreateRequest(string name)
        {
            var user = userManager.GetUserAsync(User).Result;
            var team = context.Teams.FirstOrDefault(t => t.Name == name);
            if (team == null ||
                context.TeamRequests.Any(tr => tr.Team.TeamId == team.TeamId && tr.User.Id == user.Id) ||
                team.OwnerId == user.Id)
                return RedirectToAction(nameof(ManageController.Index), "Manage",
                    new {Message = ManageController.ManageMessageId.CreateRequestError});
            context.TeamRequests.Add(new TeamRequest {Team = team, User = user});
            context.SaveChanges();
            return RedirectToAction(nameof(ManageController.Index), "Manage",
                new {Message = ManageController.ManageMessageId.CreateRequestSuccess});
        }

        [HttpPost]
        public ActionResult CancelRequest()
        {
            var user = userManager.GetUserAsync(User).Result;
            var request = context.TeamRequests.Include(r => r.User).FirstOrDefault(r => r.User.Id == user.Id);
            context.TeamRequests.Remove(request);
            context.SaveChanges();
            return RedirectToAction(nameof(ManageController.Index), "Manage");
        }

        [HttpPost]
        public ActionResult AcceptRequest(string userId)
        {
            var user = userManager.GetUserAsync(User).Result;
            var team = context.Teams.Include(t => t.Members).First(t => t.OwnerId == user.Id);
            if (team.Members.Count == team.MaxSize)
            {
                RedirectToAction(nameof(ManageController.Index), "Manage");
            }
            var request =
                context.TeamRequests.Include(r => r.Team).Include(r => r.User).FirstOrDefault(tr => tr.Team.TeamId == team.TeamId && tr.User.Id == userId);
            if (request == null)
                return RedirectToAction(nameof(ManageController.Index), "Manage");
            team.Members.Add(user);
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
            return RedirectToAction(nameof(ManageController.Index), "Manage");
        }

        [HttpGet]
        public ActionResult LeaveTeam()
        {
            var user = userManager.GetUserAsync(User).Result;
            var allTeams = context.Teams.Include(t => t.Members).ToArray();
            var team = allTeams.FirstOrDefault(t => t.Members.Any(m => m.Id == user.Id));
            if (team == null)
                return RedirectToAction(nameof(ManageController.Index), "Manage");
            user.Team = null;
            team.Members = team.Members.Where(m => m.Id != user.Id).ToList();
            context.TeamLogs.Add(new TeamLog
            {
                Action = 2,
                Team = team,
                User = user,
                Time = DateTime.Now
            });

            context.SaveChanges();
            return RedirectToAction(nameof(ManageController.Index), "Manage");
        }

        //[HttpGet]
        //[Route("Teams/Search/{teamNamePrefix}")]
        //public JsonResult Index(string teamNamePrefix)
        //{
        //    if (string.IsNullOrEmpty(teamNamePrefix))
        //        return new JsonResult(new {teams = new string[0]});
        //    var teams = context
        //        .Teams
        //        .Where(t => t.Name.StartsWith(teamNamePrefix, StringComparison.CurrentCultureIgnoreCase))
        //        .Select(t => t.Name)
        //        .Take(5)
        //        .ToArray();
        //    return new JsonResult(new {teams});
        //}

        [AllowAnonymous]
        [HttpGet]
        public JsonResult GetAllCvarcTags(string apiKey)
        {
            return new JsonResult(apiKey == WebConstants.ApiKey ? context.Teams.Select(t => t.CvarcTag).ToArray() : new Guid[0]);
        }


        [HttpPost]
        public IActionResult SetTeamName(string teamName)
        {
            var user = userManager.GetUserAsync(User).Result;
            var team = context.Teams.FirstOrDefault(t => t.OwnerId == user.Id);
            if (team == null)
                return RedirectToAction(nameof(ManageController.Index), "Manage");
            team.Name = teamName;
            context.SaveChanges();
            return RedirectToAction(nameof(ManageController.Index), "Manage");
        }
    }
}