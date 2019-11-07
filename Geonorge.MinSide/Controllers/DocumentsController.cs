using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Geonorge.MinSide.Infrastructure.Context;
using Geonorge.MinSide.Services;
using Microsoft.AspNetCore.Authorization;
using Geonorge.MinSide.Services.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using Geonorge.MinSide.Utils;
using Geonorge.MinSide.Models;

namespace Geonorge.MinSide.Web.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly IDocumentService _documentService;
        ApplicationSettings _applicationSettings;
        public DocumentsController(IDocumentService documentService, ApplicationSettings applicationSettings)
        {
            _documentService = documentService;
            _applicationSettings = applicationSettings;
        }

        // GET: Documents
        public async Task<IActionResult> Index()
        {
            return View(await _documentService.GetAll(HttpContext.Session.GetString("OrganizationNumber")));
        }

        // GET: Documents
        public async Task<IActionResult> Download(int id)
        {
            var document = await _documentService.Get(id);
            if(document == null)
                return Content("Fil finnes ikke");

            if (document.OrganizationNumber == HttpContext.Session.GetString("OrganizationNumber"))
            {
                string path = _applicationSettings.FilePath + "\\" + document.FileName;
                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, Helper.GetContentType(path), Path.GetFileName(path));
            }

            return NotFound();
        }

        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        // GET: Documents/Create
        public IActionResult Create()
        {
            Document document = new Document();
            document.Date = DateTime.Today;
            return View(document);
        }

        // POST: Documents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrganizationNumber,Type,Name,FileName,Date,Status")] Document document, IFormFile file)
        {

            var ext = Helper.GetFileExtension(file.FileName);

            if (string.IsNullOrEmpty(ext) || !Helper.PermittedFileExtensions.Contains(ext))
            {
                ModelState.AddModelError("FileName", "Ugyldig filendelse");
            }

            if (ModelState.IsValid)
            {
                await _documentService.Create(document, file);

                return RedirectToAction(nameof(Index));
            }
            return View(document);
        }

        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        // GET: Documents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _documentService.Get(id.Value);
            if (document == null)
            {
                return NotFound();
            }
            return View(document);
        }

        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrganizationNumber,Type,Name,FileName,Date,Status")] Document document)
        {
            if (id != document.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _documentService.Update(document, document.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentExists(document.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(document);
        }

        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        // GET: Documents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _documentService.Get(id.Value);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // POST: Documents/Delete/5
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var document = await _documentService.Get(id);
            await _documentService.Delete(document);
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentExists(int id)
        {
            var document = _documentService.Get(id);
            return document != null;
        }
    }
}
