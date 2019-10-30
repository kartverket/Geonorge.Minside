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

namespace Geonorge.MinSide.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View("LogIn");
            }

            ViewData["OrganizationName"] = HttpContext.User.GetOrganizationName();
            ViewData["OrganizationOrgnr"] = HttpContext.User.GetOrganizationOrgnr();

            return View();
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
