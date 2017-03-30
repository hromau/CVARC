using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CvarcWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CvarcWeb.Models;
using CvarcWeb.Models.ManageViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CvarcWeb.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger _logger;
        private readonly UserDbContext context;
        const string solutionDir = "Solutions/";

        public ManageController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILoggerFactory loggerFactory, UserDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
            _logger = loggerFactory.CreateLogger<ManageController>();
        }

        //
        // GET: /Manage/Index
        [HttpGet]
        public async Task<IActionResult> Index(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.TeamAlreadyExistsError ? "Team with this name already exists"
                : message == ManageMessageId.CreateTeamSuccess ? "Team has been created"
                : message == ManageMessageId.CreateRequestSuccess ? "Request has been created"
                : message == ManageMessageId.CreateRequestError ? "Team with this name doesn't exist or you created request in this team already"
                : message == ManageMessageId.Error ? "ERROR!"
                : "";
            if (message.ToString().EndsWith("error", StringComparison.CurrentCultureIgnoreCase))
                ViewData["IsError"] = true;
            else
                ViewData["IsError"] = false;

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var team = context.Teams.Include(t => t.Members).FirstOrDefault(t => t.Members.Any(u => u.Id == user.Id));
            var hasOwnTeam = team?.OwnerId == user.Id;
            var model = new IndexViewModel
            {
                HasPassword = await userManager.HasPasswordAsync(user),
                PhoneNumber = await userManager.GetPhoneNumberAsync(user),
                TwoFactor = await userManager.GetTwoFactorEnabledAsync(user),
                Logins = await userManager.GetLoginsAsync(user),
                BrowserRemembered = await signInManager.IsTwoFactorClientRememberedAsync(user),
                RequestsInUserTeam = GetRequestsInUserTeam(user),
                UserRequestsInOtherTeam = GetUserRequestsInOtherTeams(),
                HasOwnTeam = hasOwnTeam,
                Team = team,
                MaxSize = team?.MaxSize ?? 0,
                CanOwnerLeave = team?.CanOwnerLeave ?? false,
                HasTeam = team != null,
                HasSolution = team != null && System.IO.File.Exists(solutionDir + team.TeamId + ".zip")
            };

            return View(model);
        }

        //
        // GET: /Manage/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User changed their password successfully.");
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                AddErrors(result);
                return View(model);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        //
        // GET: /Manage/SetPassword
        [HttpGet]
        public IActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await userManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
                return View(model);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadSolution(IList<IFormFile> files)
        {
            var file = Request.Form.Files["Solution"];

            var user = await GetCurrentUserAsync();
            var team = context.Teams.First(t => t.OwnerId == user.Id);
            if (team == null)
            {
                return new ContentResult {Content = "Error. You not in team or u not creator of team."};
            }

            if (!file.FileName.EndsWith(".zip"))
            {
                return new ContentResult { Content = "Error. file extension is not zip." };
            }

            if (!Directory.Exists(solutionDir))
                Directory.CreateDirectory(solutionDir);
            using (var stream = System.IO.File.OpenWrite(solutionDir + team.TeamId + ".zip"))
                await file.CopyToAsync(stream);
            
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DownloadSolution()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var team = context.Teams.FirstOrDefault(t => t.OwnerId == user.Id) ??
                       context.Teams.FirstOrDefault(t => t.Members.Any(u => u.Id == user.Id));
            if (team != null && System.IO.File.Exists(solutionDir + team.TeamId + ".zip"))
                return File(System.IO.File.OpenRead(solutionDir + team.TeamId + ".zip"), "application/octet-stream", team.Name + ".zip");
            return RedirectToAction(nameof(Index));
        }


        private IEnumerable<TeamRequest> GetRequestsInUserTeam(ApplicationUser user)
        {
            var team = context.Teams.FirstOrDefault(t => t.OwnerId == user.Id);
            if (team == null)
                return Enumerable.Empty<TeamRequest>();
            return
                context.TeamRequests
                    .Include(r => r.Team)
                    .Include(r => r.User)
                    .Where(r => r.Team.TeamId == team.TeamId)
                    .ToArray()
                    .Select(t => new { Request = t, User = t.User })
                    .GroupBy(t => t.User.Id)
                    .Select(g => g.Last().Request);
        }

        private IEnumerable<TeamRequest> GetUserRequestsInOtherTeams()
        {
            var user = userManager.GetUserAsync(User).Result;
            return context.TeamRequests.Include(r => r.Team).Where(r => r.User.Id == user.Id);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            Error,
            CreateTeamSuccess,
            TeamAlreadyExistsError,
            CreateRequestError,
            CreateRequestSuccess
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return userManager.GetUserAsync(HttpContext.User);
        }

        #endregion
    }
}
