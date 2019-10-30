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

namespace Geonorge.MinSide.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View("LogIn");
            }

            GetOrganization();

            return View();
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
