using Microsoft.EntityFrameworkCore;

namespace Geonorge.MinSide.Infrastructure.Context
{
    public class OrganizationContext : DbContext
    {
        public OrganizationContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Agreement> Agreements { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
    }
}