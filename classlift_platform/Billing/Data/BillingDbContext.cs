using System;
using System.Collections.Generic;
using Billing.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Billing.Data;

public partial class BillingDbContext : DbContext
{
    public BillingDbContext()
    {
    }

    public BillingDbContext(DbContextOptions<BillingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Feature> Features { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<OrganizationSubscription> OrganizationSubscriptions { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Planfeature> Planfeatures { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<SubscriptionEvent> SubscriptionEvents { get; set; }

    public virtual DbSet<Subscriptionplan> Subscriptionplans { get; set; }

    public virtual DbSet<Tenantregistry> Tenantregistries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=classlift_platform;user=root;password=Mlanlan78123!", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.4.8-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Feature>(entity =>
        {
            entity.HasKey(e => e.FeatureId).HasName("PRIMARY");

            entity.ToTable("features");

            entity.HasIndex(e => e.FeatureKey, "FeatureKey").IsUnique();

            entity.Property(e => e.FeatureId).HasColumnName("FeatureID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.FeatureKey).HasMaxLength(100);
            entity.Property(e => e.FeatureName).HasMaxLength(200);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PRIMARY");

            entity.ToTable("invoices");

            entity.HasIndex(e => e.PlanId, "FK_Invoices_Plans");

            entity.HasIndex(e => e.PromotionId, "FK_Invoices_Promotions");

            entity.HasIndex(e => e.OrganizationId, "IDX_Invoices_OrganizationID");

            entity.HasIndex(e => e.OrganizationSubscriptionId, "IDX_Invoices_OrganizationSubscriptionID");

            entity.HasIndex(e => new { e.OrganizationSubscriptionId, e.BillingPeriodStart, e.BillingPeriodEnd }, "UX_Invoice_Subscription_BillingPeriod").IsUnique();

            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.DiscountAmount).HasPrecision(10, 2);
            entity.Property(e => e.GeneratedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.InvoiceStatus)
                .HasDefaultValueSql("'Pending'")
                .HasColumnType("enum('Pending','Paid','Cancelled','Overdue')");
            entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
            entity.Property(e => e.OrganizationSubscriptionId).HasColumnName("OrganizationSubscriptionID");
            entity.Property(e => e.PaidAt).HasColumnType("datetime");
            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.PricePerCoach).HasPrecision(10, 2);
            entity.Property(e => e.PromotionId).HasColumnName("PromotionID");
            entity.Property(e => e.Subtotal).HasPrecision(10, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(10, 2);

            entity.HasOne(d => d.Organization).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoices_Organizations");

            entity.HasOne(d => d.OrganizationSubscription).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.OrganizationSubscriptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoices_OrganizationSubscriptions");

            entity.HasOne(d => d.Plan).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoices_Plans");

            entity.HasOne(d => d.Promotion).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.PromotionId)
                .HasConstraintName("FK_Invoices_Promotions");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.OrganizationId).HasName("PRIMARY");

            entity.ToTable("organizations");

            entity.HasIndex(e => e.CurrentPlanId, "FK_Organizations_Plans");

            entity.HasIndex(e => e.IsActive, "IDX_Organizations_IsActive");

            entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
            entity.Property(e => e.ContactEmail).HasMaxLength(200);
            entity.Property(e => e.ContactName).HasMaxLength(200);
            entity.Property(e => e.ContactPhone).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrentPlanId).HasColumnName("CurrentPlanID");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.OrganizationName).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime");

            entity.HasOne(d => d.CurrentPlan).WithMany(p => p.Organizations)
                .HasForeignKey(d => d.CurrentPlanId)
                .HasConstraintName("FK_Organizations_Plans");
        });

        modelBuilder.Entity<OrganizationSubscription>(entity =>
        {
            entity.HasKey(e => e.OrganizationSubscriptionId).HasName("PRIMARY");

            entity.ToTable("organization_subscriptions");

            entity.HasIndex(e => e.OrganizationId, "IDX_OrgSub_OrganizationID");

            entity.HasIndex(e => e.PlanId, "IDX_OrgSub_PlanID");

            entity.HasIndex(e => e.Status, "IDX_OrgSub_Status");

            
            entity.Property(e => e.OrganizationSubscriptionId).HasColumnName("OrganizationSubscriptionID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.MinimumMonthlyPrice).HasPrecision(10, 2);
            entity.Property(e => e.MonthlyPricePerCoach).HasPrecision(10, 2);
            entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
            entity.Property(e => e.OrganizationSubscriptionscol)
                .HasMaxLength(45)
                .HasColumnName("organization_subscriptionscol");
            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'Active'")
                .HasColumnType("enum('Active','Cancelled','Expired')");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime");

            entity.HasOne(d => d.Organization).WithMany(p => p.OrganizationSubscriptions)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK_OrgSub_Organizations");

            entity.HasOne(d => d.Plan).WithMany(p => p.OrganizationSubscriptions)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrgSub_Plans");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PRIMARY");

            entity.ToTable("payments");

            entity.HasIndex(e => e.InvoiceId, "IDX_Payments_InvoiceID");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasDefaultValueSql("'CAD'");
            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.Notes).HasColumnType("text");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentProvider).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus).HasColumnType("enum('Pending','Succeeded','Failed','Refunded')");
            entity.Property(e => e.ProviderTransactionId)
                .HasMaxLength(200)
                .HasColumnName("ProviderTransactionID");

            entity.HasOne(d => d.Invoice).WithMany(p => p.Payments)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("FK_Payments_Invoices");
        });

        modelBuilder.Entity<Planfeature>(entity =>
        {
            entity.HasKey(e => e.PlanFeatureId).HasName("PRIMARY");

            entity.ToTable("planfeatures");

            entity.HasIndex(e => e.FeatureId, "FK_PlanFeatures_Features");

            entity.HasIndex(e => new { e.PlanId, e.FeatureId }, "UK_Plan_Feature").IsUnique();

            entity.Property(e => e.PlanFeatureId).HasColumnName("PlanFeatureID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.FeatureId).HasColumnName("FeatureID");
            entity.Property(e => e.PlanId).HasColumnName("PlanID");

            entity.HasOne(d => d.Feature).WithMany(p => p.Planfeatures)
                .HasForeignKey(d => d.FeatureId)
                .HasConstraintName("FK_PlanFeatures_Features");

            entity.HasOne(d => d.Plan).WithMany(p => p.Planfeatures)
                .HasForeignKey(d => d.PlanId)
                .HasConstraintName("FK_PlanFeatures_Plans");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromotionId).HasName("PRIMARY");

            entity.ToTable("promotions");

            entity.HasIndex(e => e.PlanId, "FK_Promotions_Plans");

            entity.HasIndex(e => e.IsActive, "IDX_Promotions_IsActive");

            entity.Property(e => e.PromotionId).HasColumnName("PromotionID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.DiscountType).HasColumnType("enum('Percentage','FixedAmount','PriceOverride')");
            entity.Property(e => e.DiscountValue).HasPrecision(10, 2);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.PromotionName).HasMaxLength(200);

            entity.HasOne(d => d.Plan).WithMany(p => p.Promotions)
                .HasForeignKey(d => d.PlanId)
                .HasConstraintName("FK_Promotions_Plans");
        });

        modelBuilder.Entity<SubscriptionEvent>(entity =>
        {
            entity.HasKey(e => e.SubscriptionEventId).HasName("PRIMARY");

            entity.ToTable("subscription_events");

            entity.HasIndex(e => e.NewPlanId, "FK_SubEvents_NewPlan");

            entity.HasIndex(e => e.OldPlanId, "FK_SubEvents_OldPlan");

            entity.HasIndex(e => e.EffectiveAt, "IDX_SubEvents_EffectiveAt");

            entity.HasIndex(e => e.EventType, "IDX_SubEvents_EventType");

            entity.HasIndex(e => e.OrganizationId, "IDX_SubEvents_OrganizationID");

            entity.HasIndex(e => e.OrganizationSubscriptionId, "IDX_SubEvents_SubscriptionID");

            entity.Property(e => e.SubscriptionEventId).HasColumnName("SubscriptionEventID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.EffectiveAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.EventType).HasColumnType("enum('Created','Activated','PlanChanged','Cancelled','Expired','Suspended','Reactivated')");
            entity.Property(e => e.NewPlanId).HasColumnName("NewPlanID");
            entity.Property(e => e.NewStatus).HasMaxLength(50);
            entity.Property(e => e.OldPlanId).HasColumnName("OldPlanID");
            entity.Property(e => e.OldStatus).HasMaxLength(50);
            entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
            entity.Property(e => e.OrganizationSubscriptionId).HasColumnName("OrganizationSubscriptionID");
            entity.Property(e => e.Reason).HasColumnType("text");

            entity.HasOne(d => d.NewPlan).WithMany(p => p.SubscriptionEventNewPlans)
                .HasForeignKey(d => d.NewPlanId)
                .HasConstraintName("FK_SubEvents_NewPlan");

            entity.HasOne(d => d.OldPlan).WithMany(p => p.SubscriptionEventOldPlans)
                .HasForeignKey(d => d.OldPlanId)
                .HasConstraintName("FK_SubEvents_OldPlan");

            entity.HasOne(d => d.Organization).WithMany(p => p.SubscriptionEvents)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK_SubEvents_Organizations");

            entity.HasOne(d => d.OrganizationSubscription).WithMany(p => p.SubscriptionEvents)
                .HasForeignKey(d => d.OrganizationSubscriptionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_SubEvents_Subscriptions");
        });

        modelBuilder.Entity<Subscriptionplan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PRIMARY");

            entity.ToTable("subscriptionplans");

            entity.HasIndex(e => e.IsActive, "IDX_SubscriptionPlans_IsActive");

            entity.HasIndex(e => e.PlanName, "PlanName").IsUnique();

            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.MinimumMonthlyPrice).HasPrecision(10, 2);
            entity.Property(e => e.PlanName).HasMaxLength(100);
            entity.Property(e => e.PricePerCoach).HasPrecision(10, 2);
        });

        modelBuilder.Entity<Tenantregistry>(entity =>
        {
            entity.HasKey(e => e.TenantRegistryId).HasName("PRIMARY");

            entity.ToTable("tenantregistry");

            entity.HasIndex(e => e.OrganizationId, "IDX_TenantRegistry_OrganizationID");

            entity.Property(e => e.TenantRegistryId).HasColumnName("TenantRegistryID");
            //entity.Property(e => e.ConnectionString).HasColumnType("text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.DatabaseName).HasMaxLength(200);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
            entity.Property(e => e.Subdomain).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime");

            entity.HasOne(d => d.Organization).WithMany(p => p.Tenantregistries)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK_TenantRegistry_Organizations");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
