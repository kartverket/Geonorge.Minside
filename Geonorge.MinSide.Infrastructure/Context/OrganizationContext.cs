using Microsoft.EntityFrameworkCore;

namespace Geonorge.MinSide.Infrastructure.Context
{
    public class OrganizationContext : DbContext
    {
        public OrganizationContext(DbContextOptions options) : base(options)
        {
        }

        DbSet<Agreement> AgreementDocuments { get; set; }
        DbSet<Meeting> Meetings { get; set; }
    }
}