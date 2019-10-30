using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geonorge.MinSide.Infrastructure.Context;
using Geonorge.MinSide.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Geonorge.MinSide.Services
{
    public interface IDocumentService
    {
        Task<DocumentViewModel> GetAll(string organizationNumber);
        Task<Document> Get(int documentId);
        Task<Document> Create(Document document);
        Task Update(Document updatedDocument, int documentId);
        Task Delete(Document document);
    }
    
    public class DocumentService : IDocumentService
    {
        private readonly OrganizationContext _context;

        public DocumentService(OrganizationContext context)
        {
            _context = context;
        }

        public async Task<DocumentViewModel> GetAll(string organizationNumber)
        {
            DocumentViewModel documentViewModel = new DocumentViewModel();
            documentViewModel.Drafts = await _context.Documents.Where(d => d.OrganizationNumber.Equals(organizationNumber) && d.Status == "Forslag").ToListAsync();
            documentViewModel.Valid = await _context.Documents.Where(d => d.OrganizationNumber.Equals(organizationNumber) && d.Status == "Gyldig").ToListAsync();
            documentViewModel.Superseded = await _context.Documents.Where(d => d.OrganizationNumber.Equals(organizationNumber) && d.Status == "Utgått").ToListAsync();

            return documentViewModel;
        }

        public async Task<Document> Get(int documentId)
        {
            return await _context.Documents.FirstOrDefaultAsync(d => d.Id == documentId);
        }

        public async Task<Document> Create(Document document)
        {
            _context.Documents.Add(document);
            await SaveChanges();
            return document;
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
        }
    }
}
