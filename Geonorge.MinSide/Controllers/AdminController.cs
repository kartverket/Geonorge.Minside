using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geonorge.MinSide.Models;
using Geonorge.MinSide.Services.Authorization;
using Kartverket.Geonorge.Utilities.LogEntry;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Geonorge.MinSide.Web.Controllers
{
    [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
    public class AdminController : Controller
    {
        ILogEntryService _logEntryService;
        public AdminController(ILogEntryService logEntryService)
        {
            _logEntryService = logEntryService;
        }
        // GET: Admin
        public ActionResult Edit(string operation = "", int limitNumberOfEntries = 100, bool limitCurrentApplication = true)
        {
            ViewBag.limitCurrentApplication = limitCurrentApplication;
            ViewBag.limitNumberOfEntries = limitNumberOfEntries;
            ViewBag.operation = operation;
            var logEntries = _logEntryService.GetEntries(limitNumberOfEntries, operation, limitCurrentApplication).Result;
            CodeList.UpdateOrganizations();
            return View(logEntries);
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