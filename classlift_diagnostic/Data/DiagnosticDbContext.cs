using ClassLift.Diagnostic.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLift.Diagnostic.Data;

public sealed class DiagnosticDbContext(DbContextOptions<DiagnosticDbContext> options) : DbContext(options)
{
    public DbSet<DiagnosticLead> DiagnosticLeads => Set<DiagnosticLead>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var lead = modelBuilder.Entity<DiagnosticLead>();
        lead.ToTable("diagnostic_leads");
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
        lead.Property(x => x.CostOfInactionJson).HasColumnType("json");
        lead.Property(x => x.PreviousSolutionsJson).HasColumnType("json");
        lead.Property(x => x.BuyingCriteriaJson).HasColumnType("json");
        lead.Property(x => x.RecommendedModulesJson).HasColumnType("json");
        lead.HasIndex(x => x.CreatedAt);
        lead.HasIndex(x => x.Email);
        lead.HasIndex(x => x.LeadIntent);
    }
}
