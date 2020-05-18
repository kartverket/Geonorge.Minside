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
using Geonorge.MinSide.Utils;

namespace Geonorge.MinSide.Web.Controllers
{
    [Authorize]
    public class MeetingsController : Controller
    {
        private readonly IMeetingService _meetingService;
        ApplicationSettings _applicationSettings;
        public MeetingsController(IMeetingService meeetingService, ApplicationSettings applicationSettings)
        {
            _meetingService = meeetingService;
            _applicationSettings = applicationSettings;
        }

        // GET: Meetings
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin + "," + GeonorgeRoles.MetadataEditor + "," + GeonorgeRoles.ContactPerson)]
        public async Task<IActionResult> Index(string status = null)
        {
            ViewBag.status = status;
            return View(await _meetingService.GetAll(HttpContext.Session.GetString("OrganizationNumber"), status));
        }

        // GET: Meetings/Details/5
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin + "," + GeonorgeRoles.MetadataEditor + "," + GeonorgeRoles.ContactPerson)]
        public async Task<IActionResult> Details(int? id, string status = null)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.status = status;
            var meeting = await _meetingService.Get(id.Value, status);
               
            if (meeting == null)
            {
                return NotFound();
            }

            return View(meeting);
        }

        // GET: Meetings/Create
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Meetings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrganizationNumber,Date,Type,Description,Conclusion")] Meeting meeting)
        {
            if (ModelState.IsValid)
            {
                var createdMeeting = await _meetingService.Create(meeting);
                return RedirectToAction(nameof(Edit), new { id = createdMeeting.Id} );
            }
            return View(meeting);
        }

        // GET: Meetings/Edit/5
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _meetingService.Get(id.Value);
            if (meeting == null)
            {
                return NotFound();
            }
            return View(meeting);
        }

        // POST: Meetings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrganizationNumber,Date,Type,Description,Conclusion")] Meeting meeting, List<IFormFile> files)
        {
            foreach(var file in files)
            { 
                var ext = Helper.GetFileExtension(file.FileName);

                if (string.IsNullOrEmpty(ext) || !Helper.PermittedFileExtensions.Contains(ext))
                {
                    ModelState.AddModelError("Documents", "Ugyldig filendelse");
                    break;
                }
            }

            if (id != meeting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _meetingService.Update(meeting, id, files);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeetingExists(meeting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = meeting.Id });
            }
            return View(meeting);
        }

        // GET: Meetings/Delete/5
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _meetingService.Get(id.Value);
            if (meeting == null)
            {
                return NotFound();
            }

            return View(meeting);
        }

        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        public async Task<IActionResult> DeleteFile(int meetingId, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _meetingService.DeleteFile(id.Value);

            return RedirectToAction(nameof(Edit), new { id = meetingId });
        }

        //// POST: Meetings/Delete/5
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _meetingService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Meetings/EditToDo/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize (Roles = GeonorgeRoles.MetadataAdmin + "," + GeonorgeRoles.MetadataEditor + "," + GeonorgeRoles.ContactPerson)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditToDoList(int MeetingId, List<ToDo> ToDo)
        {
              await _meetingService.UpdateToDoList(MeetingId, ToDo);

             return RedirectToAction(nameof(Index), new { meetingId = MeetingId });
        }

        private bool MeetingExists(int id)
        {
            var meeting = _meetingService.Get(id);
            return meeting != null;
        }
    }
}
