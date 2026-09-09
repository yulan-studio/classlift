using ClassLift.Diagnostic.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLift.Diagnostic.Data;

public sealed class DiagnosticDbContext(DbContextOptions<DiagnosticDbContext> options) : DbContext(options)
{
    public DbSet<DiagnosticLead> DiagnosticLeads => Set<DiagnosticLead>();
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<DemoRequest> DemoRequests => Set<DemoRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var lead = modelBuilder.Entity<DiagnosticLead>();
        lead.ToTable("diagnostic_submissions");
        lead.HasKey(x => x.Id);
        lead.Property(x => x.Id).HasColumnType("char(36)");
        lead.Property(x => x.Email).HasMaxLength(320);
        lead.Property(x => x.Name).HasMaxLength(120);
        lead.Property(x => x.Organization).HasMaxLength(200);
        lead.Property(x => x.WebsiteUrl).HasMaxLength(2048);
        lead.Property(x => x.BusinessType).HasMaxLength(100);
        lead.Property(x => x.StudentCount).HasMaxLength(30);
        lead.Property(x => x.ImplementationTimeline).HasMaxLength(50);
        lead.Property(x => x.LeadIntent).HasMaxLength(20);
        lead.Property(x => x.Classification).HasMaxLength(50);
        lead.Property(x => x.CurrentToolsJson).HasColumnType("json");
        lead.Property(x => x.ImprovementAreasJson).HasColumnType("json");
        lead.Property(x => x.TopPrioritiesJson).HasColumnType("json");
        lead.Property(x => x.CostOfInactionJson).HasColumnType("json");
        lead.Property(x => x.PreviousSolutionsJson).HasColumnType("json");
        lead.Property(x => x.BuyingCriteriaJson).HasColumnType("json");
        lead.Property(x => x.RecommendedModulesJson).HasColumnType("json");
        lead.Property(x => x.UserReportJson).HasColumnType("json");
        lead.Property(x => x.SalesReportJson).HasColumnType("json");
        lead.HasIndex(x => x.CreatedAt);
        lead.HasIndex(x => x.Email);
        lead.HasIndex(x => x.LeadIntent);

        var customer = modelBuilder.Entity<Lead>();
        customer.ToTable("leads");
        customer.HasKey(x => x.Id);
        customer.Property(x => x.Id).HasColumnType("char(36)");
        customer.Property(x => x.Email).HasMaxLength(320).IsRequired();
        customer.Property(x => x.Name).HasMaxLength(120).IsRequired();
        customer.Property(x => x.Status).HasMaxLength(30);
        customer.HasIndex(x => x.Email).IsUnique();

        var demo = modelBuilder.Entity<DemoRequest>();
        demo.ToTable("demo_requests");
        demo.HasKey(x => x.Id);
        demo.Property(x => x.Id).HasColumnType("char(36)");
        demo.Property(x => x.LeadId).HasColumnType("char(36)");
        demo.HasOne<Lead>().WithMany().HasForeignKey(x => x.LeadId).OnDelete(DeleteBehavior.Cascade);
        demo.HasIndex(x => x.CreatedAt);
        demo.HasIndex(x => x.Status);
    }
}
