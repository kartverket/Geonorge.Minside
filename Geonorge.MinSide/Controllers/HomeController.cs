using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Geonorge.MinSide.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

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

            foreach (var claim in User.Claims)
            {
                if (claim.Type == "OrganizationName" && claim.Value.Length > 0)
                    ViewData["OrganizationName"] = claim.Value;

                if (claim.Type == "OrganizationOrgnr" && claim.Value.Length > 1)
                    ViewData["OrganizationOrgnr"] = claim.Value;
            }

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
