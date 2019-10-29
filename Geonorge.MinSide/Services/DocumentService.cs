using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geonorge.MinSide.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Geonorge.MinSide.Services
{
    public interface IDocumentService
    {
        Task<IEnumerable<Document>> GetAll(string organizationNumber);
        Task<Document> Get(int documentId);
        Task<Document> Create(Document document);
        Task Update(Document updatedDocument, int documentId);
    }
    
    public class DocumentService : IDocumentService
    {
        private readonly OrganizationContext _context;

        public DocumentService(OrganizationContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Document>> GetAll(string organizationNumber)
        {
            return await _context.Documents.Where(d => d.OrganizationNumber.Equals(organizationNumber)).ToListAsync();
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
    }
}
