using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Geonorge.MinSide.Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Geonorge.MinSide.Services.Authorization;
using Geonorge.MinSide.Services;
using Geonorge.MinSide.Models;
using Microsoft.AspNetCore.Http;

namespace Geonorge.MinSide.Web.Controllers
{
    [Authorize]
    public class ToDoController : Controller
    {
        private readonly IMeetingService _meetingService;
        ApplicationSettings _applicationSettings;
        public ToDoController(IMeetingService meeetingService, ApplicationSettings applicationSettings)
        {
            _meetingService = meeetingService;
            _applicationSettings = applicationSettings;
        }

        // GET: ToDo
        public async Task<IActionResult> Index(bool? initial, string[] status, int? meetingId)
        {
            var organizationNumber = HttpContext.Session.GetString("OrganizationNumber");
            List<ToDo> meetingService = null;

            if (initial ?? false)
                status = CodeList.DefaultStatus;

            if (status == null || status.Length == 0)
                meetingService = new List<ToDo>();
            else
                meetingService = await _meetingService.GetAllTodo(organizationNumber, status, meetingId);

            ViewBag.Status = status;

            return View(meetingService);
        }

        // GET: ToDo/Create
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ToDo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,ResponsibleOrganization,Deadline,Status,Comment,Done,MeetingId")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {
                toDo.OrganizationNumber = HttpContext.Session.GetString("OrganizationNumber");
                await _meetingService.CreateToDo(toDo);
                return RedirectToAction(nameof(Index), new { meetingId = toDo.MeetingId, status= toDo.Status });
            }
            return View(toDo);
        }

        // GET: ToDo/Edit/5
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDo = await _meetingService.GetToDo(id);
            if (toDo == null)
            {
                return NotFound();
            }
            return View(toDo);
        }

        // POST: ToDo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Number,Description,ResponsibleOrganization,Deadline,Status,Comment,Done,MeetingId")] ToDo toDo)
        {
            if (id != toDo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    toDo.OrganizationNumber = HttpContext.Session.GetString("OrganizationNumber");
                    _meetingService.UpdateToDo(toDo);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoExists(toDo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { meetingId = toDo.MeetingId, status = toDo.Status });
            }
            return View(toDo);
        }

        // GET: ToDo/Delete/5
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDo = await _meetingService.GetToDo(id.Value);
            if (toDo == null)
            {
                return NotFound();
            }

            return View(toDo);
        }

        // POST: ToDo/Delete/5
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _meetingService.DeleteToDo(id);
            return RedirectToAction(nameof(Index), new { meetingId = HttpContext.Request.Form["meetingId"], initial = true });
        }

        // POST: Meetings/EditToDo/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin + "," + GeonorgeRoles.MetadataEditor)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditToDoList(int MeetingId, List<ToDo> ToDo)
        {
            await _meetingService.UpdateToDoList(MeetingId, ToDo);

            return RedirectToAction(nameof(Index), new {initial = true });
        }

        private bool ToDoExists(int id)
        {
            return _meetingService.GetToDo(id) != null;
        }
    }
}
