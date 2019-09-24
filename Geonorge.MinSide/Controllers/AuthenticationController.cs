using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace Geonorge.MinSide.Controllers
{
    public class AuthenticationController : Controller
    {
        [HttpGet("~/login")]
        public ActionResult LogIn()
        {
            // Instruct the OIDC client middleware to redirect the user agent to the identity provider.
            // Note: the authenticationType parameter must match the value configured in Startup.cs
            var redirectUrl = Url.Action(nameof(HomeController.Index), "Home");
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet("~/logout"), HttpPost("~/logout")]
        public ActionResult LogOut()
        {
            // Instruct the cookies middleware to delete the local cookie created when the user agent
            // is redirected from the identity provider after a successful authorization flow and
            // to redirect the user agent to the identity provider to sign out.
            return SignOut(new AuthenticationProperties { RedirectUri = "/signout" }, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// This is the action responding to signout callback route after logout at the identity provider
        /// </summary>
        /// <returns></returns>
        [HttpGet("~/signout")]
        public ActionResult SignOutCallback()
        {
            return Redirect("/home/loggedOut");
        }
    }
}