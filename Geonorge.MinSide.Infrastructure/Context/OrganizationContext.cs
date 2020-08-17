using Microsoft.EntityFrameworkCore;

namespace Geonorge.MinSide.Infrastructure.Context
{
    public class OrganizationContext : DbContext
    {
        public OrganizationContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Document> Documents { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public virtual DbSet<ToDo> Todo { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>()
                .HasIndex(b => b.OrganizationNumber);

            modelBuilder.Entity<Document>()
            .HasIndex(b => b.Date);

            modelBuilder.Entity<Document>()
            .HasIndex(b => b.Name);

            modelBuilder.Entity<Document>()
            .HasIndex(u => u.FileName)
                .IsUnique();

            modelBuilder.Entity<Meeting>()
                .HasIndex(b => b.OrganizationNumber);

            modelBuilder.Entity<Meeting>()
                .HasIndex(b => b.Date);

            modelBuilder.Entity<ToDo>()
                .HasIndex(b => b.OrganizationNumber);

            modelBuilder.Entity<ToDo>()
                .HasIndex(b => b.Status);

            modelBuilder.Entity<ToDo>()
                .HasIndex(b => b.Deadline);
        }

    }
}