﻿using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Geonorge.MinSide.Contstants;
using Geonorge.MinSide.Infrastructure.Context;
using Geonorge.MinSide.Models;
using Geonorge.MinSide.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Geonorge.MinSide.Services
{
    public interface IDocumentService
    {
        Task<DocumentViewModel> GetAll(string organizationNumber);
        Task<Document> Get(int documentId);
        Task<Document> Create(Document document, IFormFile file);
        Task Update(Document updatedDocument, int documentId);
        Task Delete(Document document);
        Task<InfoText> GetInfoText(string organizationNumber);
        Task<Task<int>> UpdateInfoText(InfoText text);
    }
    
    public class DocumentService : IDocumentService
    {
        private readonly OrganizationContext _context;
        ApplicationSettings _applicationSettings;

        public DocumentService(OrganizationContext context, ApplicationSettings applicationSettings)
        {
            _context = context;
            _applicationSettings = applicationSettings;
        }

        public async Task<DocumentViewModel> GetAll(string organizationNumber)
        {
            var info = await GetInfoText(organizationNumber);
            var documentViewModel = new DocumentViewModel();

            documentViewModel.InfoText = info != null && !string.IsNullOrEmpty(info.Text) ? info.Text : "";
            documentViewModel.Drafts = await _context.Documents.Where(d => d.OrganizationNumber.Equals(organizationNumber) && d.Status == "Forslag").OrderBy(d => d.Name).ToListAsync();
            documentViewModel.Valid = await _context.Documents.Where(d => d.OrganizationNumber.Equals(organizationNumber) && d.Status == "Gyldig").OrderBy(d => d.Name).ToListAsync();
            documentViewModel.Superseded = await _context.Documents.Where(d => d.OrganizationNumber.Equals(organizationNumber) && d.Status == "Utgått").OrderByDescending(d => d.Date).ToListAsync();

            return documentViewModel;
        }

        public async Task<Document> Get(int documentId)
        {
            return await _context.Documents.FirstOrDefaultAsync(d => d.Id == documentId);
        }

        public async Task<Document> Create(Document document, IFormFile file)
        {
            if (!string.IsNullOrEmpty(document.Type) && !string.IsNullOrEmpty(document.Name) && document.Type == DocumentType.GeonorgeDeldistribusjonsavtale)
                document.Name = document.Type + " – " + document.Name;
            else if (string.IsNullOrEmpty(document.Name))
                document.Name = document.Type;

            string organizationName = CodeList.Organizations[document.OrganizationNumber].ToString();
            document.FileName = Helper.CreateFileName(Helper.GetFileExtension(file.FileName), document.Name, document.Date, organizationName);

            _context.Documents.Add(document);
            await SaveChanges();

            await SaveFile(document, file);

            return document;
        }

        private async Task SaveFile(Document document, IFormFile file)
        {
            if (!Directory.Exists(_applicationSettings.FilePath))
                Directory.CreateDirectory(_applicationSettings.FilePath);

            using var fileStream = new FileStream(_applicationSettings.FilePath + "\\" + document.FileName, FileMode.Create);
            
            await file.CopyToAsync(fileStream);
        }

        public async Task Update(Document updatedDocument, int documentId)
        {
            var currentDocument = await Get(documentId);
            _context.Entry(currentDocument).CurrentValues.SetValues(updatedDocument);
            await SaveChanges();
        }

        private async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Document document)
        {
            _context.Documents.Remove(document);
            await SaveChanges();
            string file = _applicationSettings.FilePath + "\\" + document.FileName;
            if (File.Exists(file))
                File.Delete(file);
        }

        public async Task<InfoText> GetInfoText(string organizationNumber) 
        {
            return await _context.InfoTexts.Where(o => o.OrganizationNumber == organizationNumber).FirstOrDefaultAsync();
        }

        public async Task<Task<int>> UpdateInfoText(InfoText text)
        {
            var infoText = await GetInfoText(text.OrganizationNumber);


            if (infoText == null)
                _context.Add(text);
            else {
                infoText.Text = text.Text;
                _context.Update(infoText);
            }

            return _context.SaveChangesAsync();
        }
    }
}
