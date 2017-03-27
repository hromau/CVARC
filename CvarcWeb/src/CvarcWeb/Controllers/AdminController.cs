using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CvarcWeb.Data;
using CvarcWeb.Models;
using CvarcWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace CvarcWeb.Controllers
{
    [Authorize(Policy = "RequireAdminRole")]
    public class AdminController : Controller
    {
        private const string AdminEmail = "av_mironov@skbkontur.ru";
        private static readonly Random rand = new Random();
        private readonly UserDbContext context;
        private const string AdminEmail2 = "fokychuk47@ya.ru";
        private const string MathMechRole = "MathMech";
        private const string ItPlanetRole = "ItPlanet";
        private readonly IEmailSender emailSender;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AdminController(UserDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender, RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.emailSender = emailSender;
            this.roleManager = roleManager;
        }

        [AllowAnonymous]
        public string AdminReg()
        {
            if (!context.Users.Any())
            {
                RegisterAdmin(AdminEmail).Wait();
                RegisterAdmin(AdminEmail2).Wait();
                return "OK";
            }

            return "WAT R U DOIN HERE??";
        }

        public async Task<ActionResult> Index()
        {
            if (!context.Users.Any())
            {
                await RegisterAdmin(AdminEmail);
                await RegisterAdmin(AdminEmail2);
            }
            return View();
        }

        private async Task RegisterAdmin(string email)
        {
            userManager.CreateAsync(
                                new ApplicationUser { Email = email, UserName = email },
                                "1q2w3e")
                                .Wait();
            await roleManager.CreateAsync(new IdentityRole("admin"));
            await userManager.AddToRoleAsync(context.Users.First(u => u.Email == email), "admin");
        }

        [HttpPost]
        public ActionResult ChangePassword(string userName, string password)
        {
            var user = context.Users.First(u => u.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase));
            var token = userManager.GeneratePasswordResetTokenAsync(user).Result;
            userManager.ResetPasswordAsync(user, token, password).Wait();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<ActionResult> ITPlanetRegistrationFromCSV()
        {
            if (!roleManager.RoleExistsAsync(ItPlanetRole).Result)
                await roleManager.CreateAsync(new IdentityRole(ItPlanetRole));
            var file = Request.Form.Files["RegInfo"];
            var ms = new MemoryStream();
            file.CopyTo(ms);
            var content = Encoding.UTF8.GetString(ms.ToArray());
            var lines = content.Split('\n');

            var messages = new List<string>();

            foreach (var line in lines)
            {
                var msg = "";
                try
                {
                    var data = line.Trim().Split(';');
                    msg += "splited! ";
                    var passwd = GeneratePass();
                    var user = new ApplicationUser
                    {
                        FIO = data[0],
                        Email = data[1],
                        UserName = data[1]
                    };
                    msg += "user parsed! ";
                    userManager.CreateAsync(user, passwd).Wait();
                    msg += "user registered! ";
                    var regedUser = context.Users.First(u => u.Email == data[1]);
                    var team = context.Teams.Add(new Team
                    {
                        CanOwnerLeave = false,
                        MaxSize = 1,
                        OwnerId = regedUser.Id,
                        CvarcTag = Guid.NewGuid()
                    });
                    context.SaveChanges();
                    await userManager.AddToRoleAsync(user, ItPlanetRole);
                    regedUser.Team = team.Entity;
                    context.SaveChanges();
                    msg += "team registered! ";
                    emailSender.SendEmail(data[1], "IT-Planet credentials",
                        "Hello!\n" +
                        "You recieved this mail because you are participating in programming competitions on homm.ulearn.me.\n" +
                        $"Your login: {user.Email}\n" +
                        $"Your password: {passwd} \n" +
                        "Pleasant coding! And let luck always be on your side. \n <3");
                    msg += "email sent";
                }
                catch (Exception e)
                {
                    msg += e.ToString();
                    messages.Add(msg);
                    break;
                }
                messages.Add(msg);
            }

            messages.Add($"registered {messages.Count}/{lines.Length}");

            return Content(string.Join("\n", messages));
        }

        //hell-copy-paste!!1
        [HttpPost]
        public async Task<ActionResult> MathMechRegistrationFromCSV()
        {
            if (!roleManager.RoleExistsAsync(MathMechRole).Result)
                await roleManager.CreateAsync(new IdentityRole(MathMechRole));
            var file = Request.Form.Files["RegInfo"];
            var ms = new MemoryStream();
            file.CopyTo(ms);
            var content = Encoding.UTF8.GetString(ms.ToArray());
            var lines = content.Split('\n');

            var messages = new List<string>();

            foreach (var line in lines)
            {
                var msg = "";
                try
                {
                    var data = line.Trim().Split(';');
                    msg += "splited! ";
                    var passwd = GeneratePass();
                    var user = new ApplicationUser
                    {
                        FIO = data[0],
                        Email = data[1],
                        UserName = data[1]
                    };
                    msg += "user parsed! ";
                    userManager.CreateAsync(user, passwd).Wait();
                    msg += "user registered! ";
                    context.SaveChanges();
                    await userManager.AddToRoleAsync(user, MathMechRole);
                    msg += "team registered! ";
                    emailSender.SendEmail(data[1], "MathMech HOMM credentials",
                        "Hello!\n" +
                        "You recieved this mail because you are participating in programming competitions on homm.ulearn.me.\n" +
                        $"Your login: {user.Email}\n" +
                        $"Your password: {passwd}\n" +
                        "Pleasant coding! And let luck always be on your side.");
                    msg += "email sent";
                }
                catch (Exception e)
                {
                    msg += e.ToString();
                    messages.Add(msg);
                    break;
                }
                messages.Add(msg);
            }

            messages.Add($"registered {messages.Count}/{lines.Length}");

            return Content(string.Join("\n", messages));
        }

        private string GeneratePass()
        {
            var chars = "poqwieurytalsdkjfhmnbvcxzQWPERIOYWQWELKJSHADFGGMNXZBCZXMCNB82703456012";
            var pass = "";
            for (var i = 0; i < 8; i++)
                pass += chars[rand.Next(chars.Length - 1)];
            return pass;
        }
    }
}