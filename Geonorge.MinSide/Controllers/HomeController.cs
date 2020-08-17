using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Geonorge.MinSide.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;
using Geonorge.MinSide.Services.Authorization;
using Geonorge.MinSide.Web.Controllers;
using System;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Geonorge.MinSide.Infrastructure.Context;

namespace Geonorge.MinSide.Controllers
{
    public class HomeController : Controller
    {
        private readonly OrganizationContext _context;
        public HomeController(OrganizationContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                HttpContext.Session.Clear();
                return View("LogIn");
            }

            GetOrganization();
            UserSettingsViewModel UserSettings = GetUserSettings();

            return View(UserSettings);
        }

        private UserSettingsViewModel GetUserSettings()
        {
            var userName = HttpContext.User.GetUsername();
            var userSettings = _context.UserSettings.Where(u => u.Username == userName).FirstOrDefault();

            if (userSettings == null)
                return new UserSettingsViewModel { Email = HttpContext.User.GetUserEmail(), TodoReminderTime = 2 };

            UserSettingsViewModel settings = new UserSettingsViewModel();
            settings.Email = userSettings.Email;
            settings.TodoNotification = userSettings.TodoNotification.HasValue && userSettings.TodoNotification.Value == true ? true: false;
            settings.TodoReminder = userSettings.TodoReminder.HasValue && userSettings.TodoReminder.Value == true ? true : false;
            settings.TodoReminderTime = userSettings.TodoReminderTime.HasValue && userSettings.TodoReminderTime.Value > 0 ? userSettings.TodoReminderTime.Value : 0;

            return settings;

        }

        public IActionResult Settings(bool? TodoNotification, bool? TodoReminder, int? TodoReminderTime)
        {
            SetUserSettings(TodoNotification, TodoReminder, TodoReminderTime);
            return Redirect("Index");
        }

        private void SetUserSettings(bool? TodoNotification, bool? TodoReminder, int? TodoReminderTime)
        {
            var userName = HttpContext.User.GetUsername();
            var email = HttpContext.User.GetUserEmail();
            var organization = HttpContext.User.GetOrganizationName();

            var userSettings = _context.UserSettings.Where(u => u.Username == userName).FirstOrDefault();
            if (userSettings == null)
            {
                _context.UserSettings.Add(new UserSettings { Username = userName, Email = email, Organization = organization, TodoNotification = TodoNotification, TodoReminder = TodoReminder, TodoReminderTime= TodoReminderTime });
                _context.SaveChanges();
            }
            else {
                userSettings.Email = email;
                userSettings.Organization = organization;
                userSettings.TodoNotification = TodoNotification;
                userSettings.TodoReminder = TodoReminder;
                userSettings.TodoReminderTime = TodoReminderTime;
                _context.UserSettings.Update(userSettings);
                _context.SaveChanges();
            }
        }

        private void GetOrganization()
        {
            var organizationNumber = HttpContext.Session.GetString("OrganizationNumber");
            var organizationName = HttpContext.Session.GetString("OrganizationName");

            if(!string.IsNullOrEmpty(organizationNumber) && !string.IsNullOrEmpty(organizationName))
            {
                ViewData["OrganizationName"] = organizationName;
                ViewData["OrganizationOrgnr"] = organizationNumber;
            }
            else
            { 
                ViewData["OrganizationName"] = HttpContext.User.GetOrganizationName();
                ViewData["OrganizationOrgnr"] = HttpContext.User.GetOrganizationOrgnr();

                HttpContext.Session.SetString("OrganizationNumber", ViewData["OrganizationOrgnr"].ToString());
                HttpContext.Session.SetString("OrganizationName", ViewData["OrganizationName"].ToString());
            }
        }

        public IActionResult LoggedOut()
        {
            HttpContext.Session.Clear();
            return View();
        }

        [Authorize]
        public IActionResult Secure()
        {
            return View();
        }

        [Authorize]
        public IActionResult ApiDemo()
        {
            return Json(new { title="hello"});
        }
        
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
