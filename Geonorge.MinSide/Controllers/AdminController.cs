using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geonorge.MinSide.Models;
using Geonorge.MinSide.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Geonorge.MinSide.Web.Controllers
{
    [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Edit()
        {
            return View();
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string selectedOrganizationNumber)
        {
            try
            {
                if (!string.IsNullOrEmpty(selectedOrganizationNumber))
                {
                    var organizationName = CodeList.Organizations[selectedOrganizationNumber].ToString();
                    HttpContext.Session.SetString("OrganizationNumber", selectedOrganizationNumber);
                    HttpContext.Session.SetString("OrganizationName", organizationName);
                }

                return RedirectToAction("Index","Home");
            }
            catch
            {
                return View();
            }
        }
    }
}