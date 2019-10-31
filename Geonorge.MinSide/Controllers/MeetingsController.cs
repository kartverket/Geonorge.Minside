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
        public async Task<IActionResult> Index()
        {
            return View(await _meetingService.GetAll(HttpContext.Session.GetString("OrganizationNumber")));
        }

        //// GET: Meetings/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var meeting = await _context.Meetings
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (meeting == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(meeting);
        //}

        //// GET: Meetings/Create
        //[Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Meetings/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,OrganizationNumber,Date,Type,Description,Conclusion")] Meeting meeting)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(meeting);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(meeting);
        //}

        //// GET: Meetings/Edit/5
        //[Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var meeting = await _context.Meetings.FindAsync(id);
        //    if (meeting == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(meeting);
        //}

        //// POST: Meetings/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,OrganizationNumber,Date,Type,Description,Conclusion")] Meeting meeting)
        //{
        //    if (id != meeting.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(meeting);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!MeetingExists(meeting.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(meeting);
        //}

        //// GET: Meetings/Delete/5
        //[Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var meeting = await _context.Meetings
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (meeting == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(meeting);
        //}

        //// POST: Meetings/Delete/5
        //[Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var meeting = await _context.Meetings.FindAsync(id);
        //    _context.Meetings.Remove(meeting);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool MeetingExists(int id)
        //{
        //    return _context.Meetings.Any(e => e.Id == id);
        //}
    }
}
