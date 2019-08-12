using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Geonorge.MinSide.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;
using Geonorge.AuthLib.Common;

namespace Geonorge.MinSide.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                var redirectUrl = Url.Action(nameof(HomeController.Index), "Home");
                return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, OpenIdConnectDefaults.AuthenticationScheme);
            }

            ViewData["OrganizationName"] = ClaimsPrincipal.Current.GetOrganizationName();
            ViewData["OrganizationOrgnr"] = ClaimsPrincipal.Current.GetOrganizationOrgnr();

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
