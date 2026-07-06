using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Contexts;
using Core.Models;



namespace Core.Contexts
{
    public class BillingDbContext : DbContext
    {
        public BillingDbContext(DbContextOptions<BillingDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TenantRegistry> TenantRegistries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TenantRegistry>(entity =>
            {
                entity.ToTable("tenantregistry");

                entity.HasKey(e => e.TenantRegistryId);

                entity.Property(e => e.TenantRegistryId)
                    .HasColumnName("TenantRegistryId");

                entity.Property(e => e.OrganizationId)
                    .HasColumnName("OrganizationId");

                entity.Property(e => e.DatabaseName)
                    .HasColumnName("DatabaseName");

                entity.Property(e => e.Subdomain)
                    .HasColumnName("Subdomain");

                entity.Property(e => e.CustomDomain)
                    .HasColumnName("CustomDomain");

                entity.Property(e => e.IsActive)
                    .HasColumnName("IsActive");
            });
        }
    }
}
