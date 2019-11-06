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

namespace Geonorge.MinSide.Web.Controllers
{
    [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
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
        public async Task<IActionResult> Index(int meetingId)
        {
            return View(await _meetingService.GetAllTodo(meetingId));
        }

        // GET: ToDo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ToDo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,ResponsibleOrganization,Deadline,Status,Comment,Done,MeetingId")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {
                await _meetingService.CreateToDo(toDo);
                return RedirectToAction(nameof(Index), new { meetingId = toDo.MeetingId });
            }
            return View(toDo);
        }

        // GET: ToDo/Edit/5
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
                return RedirectToAction(nameof(Index), new { meetingId = toDo.MeetingId });
            }
            return View(toDo);
        }

        // GET: ToDo/Delete/5
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _meetingService.DeleteToDo(id);
            return RedirectToAction(nameof(Index), new { meetingId = HttpContext.Request.Form["meetingId"] });
        }

        private bool ToDoExists(int id)
        {
            return _meetingService.GetToDo(id) != null;
        }
    }
}
