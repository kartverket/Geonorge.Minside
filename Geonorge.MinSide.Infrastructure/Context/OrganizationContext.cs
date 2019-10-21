﻿using Microsoft.EntityFrameworkCore;

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
    }
}